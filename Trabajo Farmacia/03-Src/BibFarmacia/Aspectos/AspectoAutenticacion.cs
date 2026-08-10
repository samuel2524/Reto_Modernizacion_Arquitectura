using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BibFarmacia.Clases;

namespace BibFarmacia.Aspectos
{
    public static class AspectoAutenticacion
    {
        public static bool Login(
            List<Usuario> usuarios,
            string user,
            string password)
        {
            return usuarios.Any(u =>
                u.UserName == user &&
                u.Password == password);
        }
    }
}