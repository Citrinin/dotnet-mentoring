using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UltimateInjector;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {

            var container = new Container();
            container.AddAssembly(Assembly.GetExecutingAssembly());

            var x = container.CreateInstance<ICustomer>();
            Console.WriteLine(typeof(ICustomer).IsAssignableFrom(typeof(Customer)));
            Console.WriteLine(typeof(Person).IsAssignableFrom(typeof(ChildOfPerson)));
            Console.ReadKey();
        }
    }

    [Export(typeof(ICustomer))]
    public class Customer : ICustomer
    {
        public Customer()
        {

        }

        public Customer(Shop shop, Person p)
        {

        }

        public Customer(MyFavoriteClass favoriteClass, Product product, Shop shop, Person person)
        {

        }

        public Customer(Product product, Shop shop, Person person)
        {
            Product = product;
            Shop = shop;
            Person = person;
        }

        public Person Person { get; set; }
        public Shop Shop { get; set; }
        public Product Product { get; set; }
        public MyFavoriteClass MyFavoriteClass { get; set; }
        public TinyClass TinyClass { get; set; }
    }

    public interface ICustomer { }

    [Export]
    public class Person { }

    public class ChildOfPerson : Person
    {

    }

    [Export]
    public class Product { }

    [Export]
    public class Shop { }

    public class MyFavoriteClass { }

    public class TinyClass { }


    public class PropertyInjectionClass
    {
        [Import]
        public TinyClass TinyClass { get; set; }

    }

    public class TrickyClass
    {
        [ImportConstructor]
        public TrickyClass(Shop shop)
        {

        }
    }

}
