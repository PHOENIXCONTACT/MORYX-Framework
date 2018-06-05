namespace Marvin.Runtime.Kernel
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.MarvinProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.MarvinInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // MarvinProcessInstaller
            // 
            this.MarvinProcessInstaller.Password = null;
            this.MarvinProcessInstaller.Username = null;
            // 
            // MarvinInstaller
            // 
            this.MarvinInstaller.Description = string.Format("{0} build on MaRVIN Runtime", Platform.Current.ProductName);
            this.MarvinInstaller.ServiceName = Platform.Current.ProductName;
            this.MarvinInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.MarvinProcessInstaller,
            this.MarvinInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller MarvinProcessInstaller;
        private System.ServiceProcess.ServiceInstaller MarvinInstaller;
    }
}