using Firebase.Auth;
using Firebase.Storage;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;
using RealState.Models;
using RealState.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RealState.Controllers
{
    public class StatesController : Controller
    {
        //firebase
        private static string Apikey = "AIzaSyBmuTMtbd9-jCEH55r7eAvI1YC0NC-wLXY";
        private static string Bucket = "realestate-e6dce.appspot.com";
        private static string AuthEmail = "asramhmd130@gmail.com";
        private static string Authpassword = "esraa777";

        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "iadmfN20Fsw6gtWkpeA2ZICLuWDrRIqTDNGLUQLb",
            BasePath = "https://realestate-e6dce-default-rtdb.firebaseio.com/"
        };

        public IFirebaseConfig Config { get => config; set => config = value; }
        IFirebaseClient client;


        // GET: States
        public ActionResult States(string searchBy, String Searching, int? page)
        {

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("States");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<State>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<State>(((JProperty)item).Value.ToString()));
            }
            var states = list.ToPagedList(page ?? 1, 3);
            if (searchBy == "Price")
            {
                var filter = list.Where(x => x.Price.ToString().Contains(Searching));
                return View(filter.ToList().ToPagedList(page ?? 1, filter.Count()));
            }
            else if(searchBy == "Area")
            {
                var filter = list.Where(x => x.Area.ToString().Contains(Searching));
                return View(filter.ToList().ToPagedList(page ?? 1, filter.Count()));
            }
            else if (searchBy == "City")
            {
                var filter = list.Where(x => x.City.ToString().Contains(Searching));
                return View(filter.ToList().ToPagedList(page ?? 1, filter.Count()));
            }
            else if (searchBy == "buy")
            {
                var filter = list.Where(x => x.BuyRent.ToString().Contains(Searching));
                return View(filter.ToList().ToPagedList(page ?? 1, filter.Count()));
            }
            else if (searchBy == "rent")
            {
                var filter = list.Where(x => x.BuyRent.ToString().Contains(Searching));
                return View(filter.ToList().ToPagedList(page ?? 1, filter.Count()));
            }
            else if (searchBy == "offers")
            {
                var filter = list.Where(x => x.Offers.ToString().Contains(Searching));
                return View(filter.ToList().ToPagedList(page ?? 1, filter.Count()));
            }
            else
            {
                return View(states);
            }
        }

        //details
        [HttpGet]
        public ActionResult Details(String id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("States/" + id);

            State state = JsonConvert.DeserializeObject<State>(response.Body);
            return View(state);
        }

        public ActionResult CreateState()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateState(State state, HttpPostedFileBase file)
        {
            FileStream stream;
            string link = null;
            //if submit img
            if (file.ContentLength > 0)
            {
                Random rnd = new Random();
                int num = rnd.Next();
                string path = Path.Combine(Server.MapPath("~/Content/images/"), file.FileName+num);
                file.SaveAs(path);
                stream = new FileStream(Path.Combine(path), FileMode.Open);

                var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(Apikey));
                var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, Authpassword);

                // you can use CancellationTokenSource to cancel the upload midway
                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                    })
                    .Child("images")
                    .Child(file.FileName)
                    .PutAsync(stream, cancellation.Token);
                try
                {
                    // error during upload will be thrown when you await the task
                    link = await task;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception was thrown: {0}", ex);
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////////
            try
            {
                state.link = link;
                AddStateToFirebase(state);
                ModelState.AddModelError(string.Empty, "Added Successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View();
        }

        private void AddStateToFirebase(State data)
        {
            client = new FireSharp.FirebaseClient(config);

            PushResponse response = client.Push("States/", data);
            data.State_Id = response.Result.name;
            SetResponse setResponse = client.Set("States/" + data.State_Id, data);
        }
    }
}