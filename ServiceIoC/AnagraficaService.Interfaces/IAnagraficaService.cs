using Core.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnagraficaService.Interfaces
{
    public interface IAnagraficaService
    {
        Task<IEnumerable<FrazionarioInfo>> GetFrazionariAsync();
    }
}
