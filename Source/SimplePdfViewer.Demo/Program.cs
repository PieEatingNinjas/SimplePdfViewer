using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;

namespace SimplePdfViewer.Demo
{
    public static class Program
    {
        // This project includes DISABLE_XAML_GENERATED_MAIN in the build properties,
        // which prevents the build system from generating the default Main method:
        //static void Main(string[] args)
        //{
        //    global::Windows.UI.Xaml.Application.Start((p) => new App());
        //}

        // This example code shows how you could implement the required Main method to
        // support multi-instance redirection. The minimum requirement is to call
        // Application.Start with a new App object. Beyond that, you may delete the
        // rest of the example code and replace it with your custom code if you wish.

        static void Main(string[] args)
        {
            // First, we'll get our activation event args, which are typically richer
            // than the incoming command-line args. We can use these in our app-defined
            // logic for generating the key for this instance.
            IActivatedEventArgs activatedArgs = AppInstance.GetActivatedEventArgs();

            if(activatedArgs is FileActivatedEventArgs fileArgs)
            {
                IStorageItem file = fileArgs.Files.FirstOrDefault();
                if(file != null)
                {
                    var key = file.Path;

                    //Register for this key (path of the file) or get instance that was already registered with this key.
                    var instance = AppInstance.FindOrRegisterInstanceForKey(key);
                    if (instance.IsCurrentInstance)
                    {
                        // If we successfully registered this instance, we can now just
                        // go ahead and do normal XAML initialization.
                        global::Windows.UI.Xaml.Application.Start((p) => new App());
                    }
                    else
                    {
                        // Some other instance has registered for this key, so we'll 
                        // redirect this activation to that instance instead.
                        instance.RedirectActivationTo();
                    }
                }
            }
        }
    }
}
