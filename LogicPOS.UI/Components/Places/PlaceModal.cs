﻿using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.PriceTypes.GetAllPriceTypes;
using LogicPOS.Api.Features.MovementTypes.GetAllMovementTypes;
using LogicPOS.Api.Features.Places.AddPlace;
using LogicPOS.Api.Features.Places.UpdatePlace;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class PlaceModal : EntityModal<Place>
    {
        public PlaceModal(EntityModalMode modalMode, Place entity = null) : base(modalMode, entity)
        {
        }

        private IEnumerable<PriceType> GetPriceTypes()
        {
            var getPriceTypesResult = _mediator.Send(new GetAllPriceTypesQuery()).Result;

            if (getPriceTypesResult.IsError)
            {
                return Enumerable.Empty<PriceType>();
            }

            return getPriceTypesResult.Value;
        }

        private IEnumerable<MovementType> GetMovementTypes()
        {

            var getMovementTypesResult = _mediator.Send(new GetAllMovementTypesQuery()).Result;

            if (getMovementTypesResult.IsError)
            {
                return Enumerable.Empty<MovementType>();
            }

            return getMovementTypesResult.Value;
        }

        protected override void ShowEntityData()
        {
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtOrder.Text = _entity.Order.ToString();
            _txtNotes.Value.Text = _entity.Notes;
            _checkDisabled.Active = _entity.IsDeleted;
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

        private UpdatePlaceCommand CreateUpdateCommand()
        {
            return new UpdatePlaceCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active,
                NewPriceTypeId = _comboPriceTypes.SelectedEntity.Id,
                NewMovementTypeId = _comboMovementTypes.SelectedEntity.Id
            };
        }

        private AddPlaceCommand CreateAddCommand()
        {
            return new AddPlaceCommand
            {
                Designation = _txtDesignation.Text,
                Notes = _txtNotes.Value.Text,
                PriceTypeId = _comboPriceTypes.SelectedEntity.Id,
                MovementTypeId = _comboMovementTypes.SelectedEntity.Id
            };
        }

        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());


    }
}