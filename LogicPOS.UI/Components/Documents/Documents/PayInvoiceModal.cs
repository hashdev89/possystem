﻿using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using Patagames.Pdf.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public class PayInvoiceModal : Modal
    {
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
       
        public PageTextBox TxtPaymentMethod { get; private set; }
        public PageTextBox TxtCurrency { get; private set; }
        public PageTextBox TxtExchangeRate { get; private set; }
        public PageTextBox TxtTotalPaid { get; private set; }
        public PageTextBox TxtRealTotalPaid { get; private set; }
        public PageTextBox TxtDateTime { get; private set; }
        public PageTextBox TxtNotes { get; private set; }
        private List<IValidatableField> ValidatableFields { get; set; } = new List<IValidatableField>();
        public  List<Document> Invoices { get; } = new List<Document>();
        private readonly decimal _invoicesTotalFinal;
        private string TitleBase { get; set; }

        public PayInvoiceModal(Window parent,
                               IEnumerable<Document> invoices) : base(parent,
                                                     GeneralUtils.GetResourceByName("window_title_dialog_pay_invoices"),
                                                     new Size(500, 500),
                                                     PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_pay_invoice.png")
        {
            TitleBase = WindowSettings.Title.Text;
            Invoices.AddRange(invoices);
            SetDefaultCurrency();
            _invoicesTotalFinal = Invoices.Sum(x => x.TotalFinal);
            TxtTotalPaid.Text = _invoicesTotalFinal.ToString("0.00");
            TxtRealTotalPaid.Text = _invoicesTotalFinal.ToString("0.00");
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            var invoicesTotalFinal = Invoices.Sum(x => x.TotalFinal);
            var totalPaidPercentage = (GetTotalPaid() / invoicesTotalFinal )*100;
            totalPaidPercentage = Math.Round(totalPaidPercentage, 2);
            WindowSettings.Title.Text = $"{TitleBase} ({Invoices.Count} = {invoicesTotalFinal:0.00}) - {totalPaidPercentage}%";
        }

        private decimal GetTotalPaid()
        {
            if(TxtTotalPaid.IsValid() == false || TxtExchangeRate.IsValid() == false)
            {
                return 0;
            }

            return decimal.Parse(TxtTotalPaid.Entry.Text) * decimal.Parse(TxtExchangeRate.Text);
        }

        private void SetDefaultCurrency()
        {
            TxtCurrency.SelectedEntity = Invoices.First().Currency;
            TxtCurrency.Text = (TxtCurrency.SelectedEntity as Currency).Designation;
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        protected override Widget CreateBody()
        {
            Initialize();

            var body = new VBox(false, 2);
            body.PackStart(TxtPaymentMethod.Component, false, false, 0);
            body.PackStart(TxtCurrency.Component, false, false, 0);
            body.PackStart(TxtExchangeRate.Component, false, false, 0);
            body.PackStart(PageTextBox.CreateHbox(TxtTotalPaid, TxtRealTotalPaid), false, false, 0);
            body.PackStart(TxtDateTime.Component, false, false, 0);
            body.PackStart(TxtNotes.Component, false, false, 0);

            return body;
        }

        private void Initialize()
        {
            InitializeTxtPaymentMethod();
            InitializeTxtCurrency();
            InitializeTxtExchangeRate();
            InitializeTxtTotalPaid();
            InitializeTxtRealTotalPaid();
            InitializeTxtDateTime();
            InitializeTxtNotes();

            ValidatableFields.Add(TxtPaymentMethod);
            ValidatableFields.Add(TxtCurrency);
            ValidatableFields.Add(TxtTotalPaid);
            ValidatableFields.Add(TxtDateTime);

            AddEventHandlers();
        }

        private void InitializeTxtRealTotalPaid()
        {
            TxtRealTotalPaid = new PageTextBox(this,
                                               "",
                                               isRequired: false,
                                               isValidatable: false,
                                               includeSelectButton: false,
                                               includeKeyBoardButton: false);

            TxtRealTotalPaid.Entry.Sensitive = false;
        }

        private void InitializeTxtExchangeRate()
        {
            TxtExchangeRate = new PageTextBox(this,
                                              GeneralUtils.GetResourceByName("global_exchangerate"),
                                              isRequired: true,
                                              isValidatable: true,
                                              regex: RegularExpressions.DecimalNumber,
                                              includeSelectButton: false,
                                              includeKeyBoardButton: true);

            TxtExchangeRate.Text = "1.00";

            TxtExchangeRate.Entry.Changed += TxtExchangeRate_Changed;
        }

        private void TxtExchangeRate_Changed(object sender, EventArgs e)
        {
            if(TxtExchangeRate.IsValid())
            {
                UpdateTitle();
                UpdateRealTotalPaid();
            }
        }

        private void UpdateRealTotalPaid()
        {
            TxtRealTotalPaid.Text = GetTotalPaid().ToString("0.00");
        }

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (AllFieldsAreValid() == false)
            {
                ShowValidationErrors();
                Run();
                return;
            }
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new PageTextBox(this,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
        }

        private void InitializeTxtDateTime()
        {
            TxtDateTime = new PageTextBox(this,
                                          GeneralUtils.GetResourceByName("global_date"),
                                          isRequired: true,
                                          isValidatable: true,
                                          regex: RegexUtils.RegexDateTime,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true);

            TxtDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void InitializeTxtTotalPaid()
        {
            TxtTotalPaid = new PageTextBox(this,
                                          GeneralUtils.GetResourceByName("global_total_deliver"),
                                          isRequired: true,
                                          isValidatable: true,
                                          regex: RegularExpressions.Money,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true);

            TxtTotalPaid.Entry.Changed += TxtTotalPaid_Changed;
        }

        private void TxtTotalPaid_Changed(object sender, EventArgs e)
        {
            if (!TxtTotalPaid.IsValid() || !TxtExchangeRate.IsValid())
            {
                return;
            }

            var totalPaid = GetTotalPaid();

            if (totalPaid > _invoicesTotalFinal)
            {
                var exchangeRate = decimal.Parse(TxtExchangeRate.Text);
                TxtTotalPaid.Text = (_invoicesTotalFinal / exchangeRate).ToString("0.00");
            }

            UpdateTitle();
            UpdateRealTotalPaid();
        }


        private void InitializeTxtPaymentMethod()
        {
            TxtPaymentMethod = new PageTextBox(this,
                                               GeneralUtils.GetResourceByName("global_payment_method"),
                                               isRequired: true,
                                               isValidatable: false,
                                               includeSelectButton: true,
                                               includeKeyBoardButton: false);

            TxtPaymentMethod.Entry.IsEditable = false;

            TxtPaymentMethod.SelectEntityClicked += BtnSelectPaymentMethod_Clicked;
        }

        private void BtnSelectPaymentMethod_Clicked(object sender, EventArgs e)
        {
            var page = new PaymentMethodsPage(null, PageOptions.SelectionPageOptions);
            var selectPaymentMethodModal = new EntitySelectionModal<PaymentMethod>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectPaymentMethodModal.Run();
            selectPaymentMethodModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtPaymentMethod.Text = page.SelectedEntity.Designation;
                TxtPaymentMethod.SelectedEntity = page.SelectedEntity;
            }
        }

        private void InitializeTxtCurrency()
        {
            TxtCurrency = new PageTextBox(this,
                                          GeneralUtils.GetResourceByName("global_currency"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtCurrency.Entry.IsEditable = false;

            TxtCurrency.SelectEntityClicked += BtnSelectCurrency_Clicked;
        }

        private void BtnSelectCurrency_Clicked(object sender, EventArgs e)
        {
            var page = new CurrenciesPage(null, PageOptions.SelectionPageOptions);
            var selectCurrencyModal = new EntitySelectionModal<Currency>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectCurrencyModal.Run();
            selectCurrencyModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCurrency.Text = page.SelectedEntity.Designation;
                TxtCurrency.SelectedEntity = page.SelectedEntity;
                TxtExchangeRate.Text = page.SelectedEntity.ExchangeRate.ToString("0.00");
            }
        }

        public bool AllFieldsAreValid() => ValidatableFields.All(tab => tab.IsValid());

        protected void ShowValidationErrors() => Utilities.ShowValidationErrors(ValidatableFields);

    }
}