﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AreaAnalysis.Classes;
using Eto.Forms;
using Rhino;

namespace AreaAnalysis.Views
{ 
    public class RenameHeaderModal : BaseModal
    {
        private string _newName = null;
        private readonly TableController _controller;
        private readonly string _oldName;
        private readonly GridColumn _column;
        private readonly int _index;
        private readonly List<string> _headerNames;

        
        public RenameHeaderModal(string currentHeader, TableController tControl, GridColumn column, 
            List<string>headerNames,  int headerIndex)
        {
            //pass the controller
            _controller = tControl;

            _oldName = currentHeader;

            _column = column;

            _headerNames = headerNames;

            _index = headerIndex;

            //modal title
            Title = "Rename header";
            Label label = new Label() { Text = "Provide a unique header name for your column" };

            TextBox newHeaderName = new TextBox();
            newHeaderName.PlaceholderText = currentHeader;

            // event handler for textbox
            newHeaderName.TextChanged += (sender, e) => _newName = newHeaderName.Text;

            ModalLayout.Items.Insert(0, label);
            ModalLayout.Items.Insert(1, newHeaderName);

        }

        
        protected override void OnOKButtonClicked()
        {
            if (_headerNames.Contains(_newName))

            {
                WarningMessageModal warning =
                    new WarningMessageModal("Choose a name that has not been used for a column", "Unique Name needed");
                warning.ShowModal(this);
            }
            else if (_newName != null)
            { 
                _controller.RenameHeader(_oldName, _newName, _column, _index);
                base.OnOKButtonClicked();
            }
            else
            {
                WarningMessageModal warning =
                    new WarningMessageModal("You must provide a new name or click cancel", "Name needed");
                warning.ShowModal(this);
            }

        }
    }
}
