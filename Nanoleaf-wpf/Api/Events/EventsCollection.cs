﻿using System.Collections.Generic;

namespace Winleafs.Wpf.Api.Events
{
    public class EventsCollection
    {
        private Dictionary<string, IEvent> _events;

        public EventsCollection(Orchestrator orchestrator)
        {
            _events = new Dictionary<string, IEvent>();

            _events.Add("TestProcessEvent", new TestProcessEvent(orchestrator));
        }

        public void StopAllEvents()
        {
            foreach (var ievent in _events.Values)
            {
                ievent.StopEffect();
            }
        }
    }
}
