using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class MyOwinComponent
    {
        public MyOwinComponent(AppFunc next)
        {

        }

        public async Task Invoke(IDictionary<string, object> environment)
        {

        }
    }
}
