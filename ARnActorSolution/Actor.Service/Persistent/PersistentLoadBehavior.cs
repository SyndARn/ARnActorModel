﻿/*****************************************************************************
		               ARnActor Actor Model Library .Net
     
	 Copyright (C) {2016}  {ARn/SyndARn} 
 
 
     This program is free software; you can redistribute it and/or modify 
     it under the terms of the GNU General Public License as published by 
     the Free Software Foundation; either version 2 of the License, or 
     (at your option) any later version. 
 
 
     This program is distributed in the hope that it will be useful, 
     but WITHOUT ANY WARRANTY; without even the implied warranty of 
     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
     GNU General Public License for more details. 
 
 
     You should have received a copy of the GNU General Public License along 
     with this program; if not, write to the Free Software Foundation, Inc., 
     51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA. 
*****************************************************************************/

using Actor.Base;

namespace Actor.Service
{
    public class PersistentLoadBehavior<T> : Behavior<PersistentCommand, IActor>
    {
        private readonly IPersistentService<T> _service;
        public PersistentLoadBehavior(IPersistentService<T> service) : base()
        {
            _service = service;
            Apply = DoApply;
            Pattern = DoPattern;
        }

        private bool DoPattern(PersistentCommand command, IActor sender)
        {
            return command == PersistentCommand.Load;
        }

        private void DoApply(PersistentCommand command, IActor sender)
        {
            var load = _service.Load();
            sender.SendMessage(load);
        }
    }

}
