﻿using logicpos.datalayer.DataLayer.Xpo;

namespace LogicPOS.Data.XPO.Settings
{
    public static partial class TerminalSettings
    {
        public static pos_configurationplaceterminal LoggedTerminal { get; set; }
        public static bool HasLoggedTerminal => LoggedTerminal != null;
    }
}
