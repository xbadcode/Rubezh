﻿using System;
using System.ComponentModel;
using System.Windows.Threading;
using FireAdministrator.ViewModels;
using Infrastructure;

namespace FireAdministrator
{
    public class ProgressService : IProgressService
    {
        ProgressViewModel _progressViewModel;
        Action _work;
        Action _completed;

        public void Run(Action work, Action completed, string tite)
        {
            _work = work;
            _completed = completed;
            _progressViewModel = new ProgressViewModel(tite);

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.RunWorkerAsync();

            ServiceFactory.UserDialogs.ShowModalWindow(_progressViewModel);
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_work != null)
                _work();
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(new Action(StopProgress));

            if (_completed != null)
                _completed();
        }

        void StopProgress()
        {
            try
            {
                _progressViewModel.CloseProgress();
            }
            catch (Exception e)
            {
                ;
            }
        }
    }
}