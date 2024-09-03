﻿using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.SizeUnits.GetAllSizeUnits;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System;
using System.Collections.Generic;


namespace LogicPOS.UI.Components.Pages
{
    public class SizeUnitPage : Page<SizeUnit>
    {
        public SizeUnitPage(Window parent) : base(parent)
        {
        }


        protected override IRequest<ErrorOr<IEnumerable<SizeUnit>>> GetAllQuery => new GetAllSizeUnitsQuery();
       

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new SizeUnitModal(mode, SelectedEntity as SizeUnit);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(4));
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(5));
        }


        protected override void InitializeSort()
        {

            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddUpdatedAtSorting(2);
        }
    }
    
}