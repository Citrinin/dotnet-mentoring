using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace UltimateInjector.Tests
{
    [TestClass()]
    public class ContainerTests
    {
        #region ClassesDeclaration

        
        [Export(typeof(ICustomer))]
        public class Customer : ICustomer
        {
            public Customer(){}

            public Customer(Shop shop, Person p)
            {
                Shop = shop;
                Person = p;
            }

            public Customer(MyFavoriteClass favoriteClass, Product product, Shop shop, Person person)
            {
                MyFavoriteClass = favoriteClass;
                Product = product;
                Shop = shop;
                Person = person;
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

        public interface ICustomer
        {
            Person Person { get; set; }
            Shop Shop { get; set; }
            Product Product { get; set; }
        }

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
            public Person Person { get; set; }
            
        }

        public class DummyPropertyInjectionClass
        {
            [Import]
            public TinyClass TinyClass { get; set; }

        }

        public class TrickyClass
        {
            [ImportConstructor]
            public TrickyClass(Shop shop)
            {
                Shop = shop;
            }

            public TrickyClass(Person person, Customer customer)
            {
                Customer = customer;
                Person = person;
            }

            public Shop Shop { get; set; }
            public Person Person { get; set; }
            public Customer Customer { get; set; }

        }

        public abstract class AbstractClass
        {
            
        }

        public class WrongConstructorInjection
        {
            [ImportConstructor]
            public WrongConstructorInjection(MyFavoriteClass myFavorite)
            {
                
            }
        }

        [Export]
        public class CyclicDependency
        {
            public CyclicDependency(SecondCyclicDependency secondCyclicDependency)
            {
                
            }
        }

        [Export]
        public class SecondCyclicDependency
        {
            public SecondCyclicDependency(CyclicDependency cyclicDependency)
            {

            }
        }

        #endregion

        [TestMethod()]
        public void CreateInstanceTest()
        {
            var container = new Container();
            container.AddType(typeof(Person));
            var person = container.CreateInstance(typeof(Person));
            Assert.IsNotNull(person);
        }

        [TestMethod()]
        public void CreateInstanceGenericTest()
        {
            var container = new Container();
            container.AddType(typeof(Person));
            var person = container.CreateInstance<Person>();
            Assert.IsNotNull(person);
        }

        [TestMethod()]
        public void CreateInstanceTestRegisterOnInterface()
        {
            var container = new Container();
            container.AddType(typeof(Customer),typeof(ICustomer));
            var customer = container.CreateInstance(typeof(ICustomer));
            Assert.IsNotNull(customer);
            Assert.IsTrue(customer is ICustomer);
        }

        [TestMethod()]
        public void CreateInstanceGenericTestRegisterOnInterface()
        {
            var container = new Container();
            container.AddType(typeof(Customer), typeof(ICustomer));
            var customer = container.CreateInstance<ICustomer>();
            Assert.IsNotNull(customer);
            Assert.IsTrue(customer is Customer);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateInstanceTestNotRegistered()
        {
            var container = new Container();
            container.CreateInstance<ICustomer>();
        }

        [TestMethod()]
        [ExpectedException(typeof(TypeIsNotAssignedException))]
        public void AddTypeTestClassDoesNotImplementInterface()
        {
            var container = new Container();
            container.AddType(typeof(Person), typeof(ICustomer));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void AddTypeTestAbstractClass()
        {
            var container = new Container();
            container.AddType(typeof(AbstractClass));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void AddTypeTestInterface()
        {
            var container = new Container();
            container.AddType(typeof(ICustomer));
        }

        [TestMethod()]
        public void AddAssemblyTest()
        {
            var container = new Container();
            container.AddAssembly(Assembly.GetExecutingAssembly());
            Assert.IsNotNull(container.CreateInstance<Shop>());
        }

        [TestMethod()]
        public void ExportAttributeTest()
        {
            var container = new Container();
            container.AddAssembly(Assembly.GetExecutingAssembly());
            var customer = container.CreateInstance<ICustomer>();
            Assert.IsNotNull(customer);
            Assert.IsNotNull(customer.Person);
            Assert.IsNotNull(customer.Product);
            Assert.IsNotNull(customer.Shop);
        }

        [TestMethod()]
        public void ImportConstructorAttributeTest()
        {
            var container = new Container();
            container.AddAssembly(Assembly.GetExecutingAssembly());
            var tricky = container.CreateInstance<TrickyClass>();
            Assert.IsNotNull(tricky);
            Assert.IsNotNull(tricky.Shop);
            Assert.IsNull(tricky.Person);
            Assert.IsNull(tricky.Customer);
        }

        [TestMethod()]
        [ExpectedException(typeof(InjectionException))]
        public void ImportConstructorAttributeTestNotAllParametersRegistered()
        {
            var container = new Container();
            container.AddAssembly(Assembly.GetExecutingAssembly());
            container.CreateInstance<WrongConstructorInjection>();

        }

        [TestMethod()]
        public void ImportAttributeTest()
        {
            var container = new Container();
            container.AddAssembly(Assembly.GetExecutingAssembly());
            var propertyInjection = container.CreateInstance<PropertyInjectionClass>();
            Assert.IsNotNull(propertyInjection);
            Assert.IsNotNull(propertyInjection.Person);
        }

        [TestMethod()]
        [ExpectedException(typeof(InjectionException))]
        public void ImportAttributeTestNotRegisteredExeption()
        {
            var container = new Container();
            container.AddAssembly(Assembly.GetExecutingAssembly());
            container.CreateInstance<DummyPropertyInjectionClass>();
        }

        [TestMethod()]
        [ExpectedException(typeof(InjectionException))]
        public void ExportAttributeTestCyclicDependency()
        {
            var container = new Container();
            container.AddAssembly(Assembly.GetExecutingAssembly());
            container.CreateInstance<CyclicDependency>();
        }
    }
}