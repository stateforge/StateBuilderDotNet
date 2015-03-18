using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace StateForge.Examples.Wp7Blink
{
    public partial class App : Application
    {
        /// <summary>
        /// Offre facile accesso al frame radice dell'applicazione Windows Phone.
        /// </summary>
        /// <returns>Nome radice dell'applicazione Windows Phone.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Costruttore dell'oggetto Application.
        /// </summary>
        public App()
        {
            // Gestore globale delle eccezioni non rilevate. 
            UnhandledException += Application_UnhandledException;

            // Inizializzazione Silverlight standard
            InitializeComponent();

            // Inizializzazione specifica del telefono
            InitializePhoneApplication();

            // Visualizza informazioni di profilatura delle immagini durante il debug.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Visualizza i contatori della frequenza fotogrammi corrente.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Visualizza le aree dell'applicazione che vengono ridisegnate in ogni fotogramma.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Abilitare la modalità di visualizzazione dell'analisi non di produzione, 
                // che consente di visualizzare le aree di una pagina passate alla GPU con una sovrapposizione colorata.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disabilitare il rilevamento dell'inattività dell'applicazione impostando la proprietà UserIdleDetectionMode
                // dell'oggetto PhoneApplicationService dell'applicazione su Disabled.
                // Attenzione: utilizzare questa opzione solo in modalità di debug. L'applicazione che disabilita il rilevamento dell'inattività dell'utente continuerà ad essere eseguita
                // e a consumare energia quando l'utente non utilizza il telefono.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Codice da eseguire all'avvio dell'applicazione (ad esempio da Start)
        // Questo codice non verrà eseguito quando l'applicazione viene riattivata
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Codice da eseguire quando l'applicazione viene attivata (portata in primo piano)
        // Questo codice non verrà eseguito al primo avvio dell'applicazione
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Codice da eseguire quando l'applicazione viene disattivata (inviata in background)
        // Questo codice non verrà eseguito alla chiusura dell'applicazione
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Codice da eseguire alla chiusura dell'applicazione (ad esempio se l'utente fa clic su Indietro)
        // Questo codice non verrà eseguito quando l'applicazione viene disattivata
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Codice da eseguire se un'operazione di navigazione ha esito negativo
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Un'operazione di navigazione ha avuto esito negativo; inserire un'interruzione nel debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Codice da eseguire in caso di eccezioni non gestite
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Si è verificata un'eccezione non gestita; inserire un'interruzione nel debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Inizializzazione dell'applicazione Windows Phone

        // Evitare la doppia inizializzazione
        private bool phoneApplicationInitialized = false;

        // Non aggiungere altro codice a questo metodo
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Creare il fotogramma ma non impostarlo ancora come RootVisual; in questo modo
            // la schermata iniziale rimane attiva finché non viene completata la preparazione al rendering dell'applicazione.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Gestisce gli errori di navigazione
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Accertarsi che l'inizializzazione non venga ripetuta
            phoneApplicationInitialized = true;
        }

        // Non aggiungere altro codice a questo metodo
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Impostare l'elemento visivo radice per consentire il rendering dell'applicazione
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Rimuovere il gestore in quanto non più necessario
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}