using System;
using ServiceReference1;

namespace AbstractProject
{
    class Program
    {
        static void Main(string[] args)
        {

            MernisServiceAdapter m = new MernisServiceAdapter();
            Customer c = new Customer
            {
                FirstName = "Tolgahan",BirthTime = "1996",LastName ="Erbabi",NationalityId = "23266798474"
            };

            StarbucksManager s = new StarbucksManager(m);
            s.CustomerAdd(c);
        }
    }


    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalityId { get; set; }
        public string BirthTime { get; set; }
    }

    public interface IService
    {
        void CustomerAdd(Customer customer);
    }

    public abstract class Manager : IService
    {
        public virtual void CustomerAdd(Customer customer)
        {
            Console.WriteLine($"{customer.FirstName} Kullanıcısı Veri tabanına eklendi");
        }
    }

    public interface IMernisService
    {
        bool checkToMernis(Customer customer);
    }

    public class MernisManager : IMernisService
    {
        public bool checkToMernis(Customer customer)
        {
            return true;
        }
    }

    // Adapter Design Pattern
    public class MernisServiceAdapter:IMernisService
    {
        public bool checkToMernis(Customer customer)
        {
            KPSPublicSoapClient client = new KPSPublicSoapClient(KPSPublicSoapClient.EndpointConfiguration.KPSPublicSoap);
            return client.TCKimlikNoDogrulaAsync(new TCKimlikNoDogrulaRequest(new TCKimlikNoDogrulaRequestBody(Convert.ToInt64(customer.NationalityId), customer.FirstName.ToUpper(), customer.LastName.ToUpper(), Convert.ToInt32(customer.BirthTime)))).Result.Body.TCKimlikNoDogrulaResult;
        }
    }

    public class StarbucksManager : Manager
    {
        private IMernisService _mernisService;

        public StarbucksManager(IMernisService mernisService)
        {
            _mernisService = mernisService;
        }

        public override void CustomerAdd(Customer customer)
        {
            if (_mernisService.checkToMernis(customer))
            {
                
                base.CustomerAdd(customer);
            }
            else
            {
                throw new Exception("Kullanıcı Bulunamadı");
            }
        }
    }

    public class NeroManager : Manager
    {
    }
}
