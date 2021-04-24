﻿using System;

namespace Stratego.Controler.Network
{
    public class StringEventArgs : EventArgs
    {
        public String Data { get; set; }

        public StringEventArgs(String data)
        {
            this.Data = data;
        }

        public override String ToString()
        {
            return Data;
        }
    }
}
