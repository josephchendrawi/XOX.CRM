using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using XOX.CRM.Lib.Services;

namespace XOX.CRM.Scheduler
{
    public partial class Service : ServiceBase
    {
        private const int TIMER_INTERVAL = 20000; //20 seconds

        private Timer timer = null;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
           (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private BatchWorkService BatchWorkService = new BatchWorkService();

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();

            timer = new Timer();
            this.timer.Interval = TIMER_INTERVAL;
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Tick);
            timer.Enabled = true;
            
            log.Info("XOX.CRM.Scheduler started");
        }

        private void timer_Tick(object sender, ElapsedEventArgs e)
        {
            //log.Info("Timer ticked");
            try
            {
                BatchWorkService.CheckAndDoBatchWork();
            }
            catch (Exception ex)
            {
                var trace = new StackTrace(ex, true);
                var frame = trace.GetFrame(0);

                log.Error("at " + frame.GetMethod().Name + " line:" + frame.GetFileLineNumber() + " - " + ex.Message + "; " + ex.InnerException);        
            }
        }

        protected override void OnStop()
        {
            log.Info("XOX.CRM.Scheduler stopped");
        }
    }
}
