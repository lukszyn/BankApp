using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BankApp.BusinessLayer
{
    public class TimerService
    {
        private Timer _timer;
        private Action _timeElapsed;

        public void SetTimer(Action action, double time)
        {
            _timeElapsed = action;
            _timer = new Timer(time);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            _timeElapsed?.Invoke();
        }
    }
}
