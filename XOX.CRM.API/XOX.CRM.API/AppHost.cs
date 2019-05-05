using Funq;
using Microsoft.Practices.Unity;
using ServiceStack;
using ServiceStack.Configuration;
using XOX.CRM.API.ServiceInterface;
using XOX.CRM.API.ServiceModel.Types;
using XOX.CRM.Lib;

namespace XOX.CRM.API
{
    public class AppHost : AppHostBase
    {
        /// <summary>
        /// Default constructor.
        /// Base constructor requires a name and assembly to locate web service classes. 
        /// </summary>
        public AppHost()
            : base("XOX.CRM.API", typeof(MyServices).Assembly)
        {

        }

        /// <summary>
        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        /// </summary>
        /// <param name="container"></param>
        public override void Configure(Container container)
        {
            //Config examples
            //this.Plugins.Add(new PostmanFeature());
            //this.Plugins.Add(new CorsFeature());
            Plugins.Add(new SessionFeature());

            //Create the IUnityContainer
            IUnityContainer unityContainer = new UnityContainer();
            //register your objects
            unityContainer.RegisterType<IAccountService, AccountService>();
            unityContainer.RegisterType<IAccountAttachmentService, AccountAttachmentService>();
            unityContainer.RegisterType<IOrderService, OrderService>();
            unityContainer.RegisterType<IProductService, ProductService>();

            //Create the Adapter
            var unityAdapter = new UnityContainerAdapter(unityContainer);
            //Add the new adapter to ServiceStack
            container.Adapter = unityAdapter;

            this.PreRequestFilters.Add((req, res) =>
            {
                //
                AuthUserSession session = (AuthUserSession)req.GetSession();
                session.UserAuthId = "1";
                session.RequestTokenSecret = "";

                req.SaveSession(session);
                //

                //if (!req.GetAbsolutePath().ToLower().Contains("/metadata"))
                //{
                //    AuthUserSession session = (AuthUserSession)req.GetSession();
                //    if (req.GetHeader("Key") == null || req.GetHeader("Key") == "" || req.GetHeader("UserID") == null || req.GetHeader("UserID") == "")
                //    {
                //        throw new AuthenticationException();
                //    }
                //    else
                //    {
                //        try
                //        {
                //            bool result = APIService.isAuth(long.Parse(req.GetHeader("UserID")), req.GetHeader("Key"));
                //            if (result == false)
                //                throw new AuthenticationException();

                //            session.UserAuthId = req.GetHeader("UserID");
                //            session.RequestTokenSecret = req.GetHeader("Key");

                //            req.SaveSession(session);
                //        }
                //        catch
                //        {
                //            throw new AuthenticationException();
                //        }
                //    }
                //}
            });

            this.GlobalResponseFilters.Add((req, res, dto) =>
            {
                req.RemoveSession();
            });
            
            this.ServiceExceptionHandlers.Add((httpReq, request, exception) =>
            {
                string InnerException = "";
                try
                {
                    if (exception.InnerException != null)
                    {
                        InnerException = "--> InnerException : " + exception.InnerException.Message;
                        if (exception.InnerException.InnerException != null)
                        {
                            InnerException += exception.InnerException.InnerException.Message;
                        }
                    }
                }
                catch { }

                string ExceptionDetail = "";
                try
                {
                    ExceptionDetail = exception.Message + "\n" + exception.StackTrace + "\n" + exception.Source + InnerException;
                }
                catch { }

                return new ObjResponse()
                {
                    Result = exception.Message + InnerException,
                    Key = ExceptionDetail
                };
            });

        }

        /// <summary>
        /// IContainerAdapter for the unity framework.
        /// </summary>
        public class UnityContainerAdapter : IContainerAdapter
        {
            private readonly IUnityContainer _unityContainer;

            public UnityContainerAdapter(IUnityContainer container)
            {
                _unityContainer = container;
            }

            public T Resolve<T>()
            {
                return _unityContainer.Resolve<T>();
            }

            public T TryResolve<T>()
            {
                if (_unityContainer.IsRegistered(typeof(T)))
                {
                    return (T)_unityContainer.Resolve(typeof(T));
                }

                return default(T);
            }
        }
    }
}