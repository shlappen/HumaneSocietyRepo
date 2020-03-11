using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;
        public delegate Animal ToAnimalFunction(string s);

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
        


        //////////////READ
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }


        /////////////CREATE
        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }


        ///////////////UPDATE
        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }



        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }






        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            if (crudOperation == "create")
            {
                db.Employees.InsertOnSubmit(employee);
                db.SubmitChanges();
            }
            else if (crudOperation == "read")
            {
                UserInterface.DisplayEmployeeInfo(employee = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).Single());
            }
            else if (crudOperation == "update")
            {
                Employee employeeFromDb = null;
                employeeFromDb = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).Single();
                employeeFromDb.FirstName = employee.FirstName;
                employeeFromDb.LastName = employee.LastName;
                employeeFromDb.Email = employee.Email;
                db.SubmitChanges();
            }
            else if(crudOperation == "delete")
            {
                Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).Single();
                db.Employees.DeleteOnSubmit(employeeFromDb);
                db.SubmitChanges();
            }
        }





        // Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }
        
        internal static Animal GetAnimalByID(int id)
        {
            Animal animalFromDb = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();

            if (animalFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return animalFromDb;
            }

        }
        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal animalFromDb = null;
            animalFromDb = db.Animals.Where(a => a.AnimalId == animalId).FirstOrDefault();
            //switch case for groups
            animalFromDb.CategoryId = Convert.ToInt32(updates[1]);

            animalFromDb.Name = updates[2];
            animalFromDb.Age = Convert.ToInt32(updates[3]);
            animalFromDb.Demeanor = updates[4];

            animalFromDb.KidFriendly = Convert.ToBoolean(updates[5]);
            animalFromDb.PetFriendly = Convert.ToBoolean(updates[6]);

            animalFromDb.Weight = Convert.ToInt32(updates[7]);
            animalFromDb.AnimalId = Convert.ToInt32(updates[8]);
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }
        





        // TODO Animal Multi-Trait Search
        internal static List<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            List<Animal> animals = db.Animals.ToList();

            foreach(KeyValuePair<int,string> update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        animals = animals.Where(s => s.CategoryId == Convert.ToInt32(update.Value)).ToList();
                        break;
                    case 2:
                        animals = animals.Where(s => s.Name == (update.Value)).ToList();
                        break;
                    case 3:
                        animals = animals.Where(s => s.Age == Convert.ToInt32(update.Value)).ToList();
                        break;
                    case 4:
                        animals = animals.Where(s => s.Demeanor == (update.Value)).ToList();
                        break;
                    case 5:
                        animals = animals.Where(s => s.KidFriendly == Convert.ToBoolean(update.Value)).ToList();
                        break;
                    case 6:
                        animals = animals.Where(s => s.PetFriendly == Convert.ToBoolean(update.Value)).ToList();
                        break;
                    case 7:
                        animals = animals.Where(s => s.Weight == Convert.ToInt32(update.Value)).ToList();
                        break;
                    case 8:
                        animals = animals.Where(s => s.AnimalId == Convert.ToInt32(update.Value)).ToList();
                        break;
                    default:
                        break;
                }
            }
            return animals;
        }
         
        internal static int GetCategoryId(string categoryName)
        {
            var categoryId = db.Categories.Where(c => c.Name == categoryName).Select(c => c.CategoryId).SingleOrDefault();
            return categoryId;
        }
        
        internal static Room GetRoom(int animalId)
        {
            Room roomFromDb = db.Rooms.Where(n => n.AnimalId == animalId).FirstOrDefault();
            return roomFromDb;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            var dietPlanId = db.DietPlans.Where(n => n.Name == dietPlanName).Select(n => n.DietPlanId).FirstOrDefault();
            return Convert.ToInt32(dietPlanId);
        }






        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = null;

            adoption.ClientId = client.ClientId;
            adoption.AnimalId = animal.AnimalId;
            adoption.ApprovalStatus = "Pending";
            adoption.AdoptionFee = 75;
            adoption.PaymentCollected = false;


            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {

            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            Adoption updatedAdoption = null;
            //find animalID
            try
            {
                updatedAdoption = db.Adoptions.Where(a => a.AnimalId == adoption.AnimalId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Did not find animal that matched.");
                Console.WriteLine("No update(s) have been made.");
                return;
            }

            updatedAdoption.AnimalId = adoption.AnimalId; 
            updatedAdoption.ClientId = adoption.ClientId;
            updatedAdoption.ApprovalStatus = "Adopted";
            updatedAdoption.AdoptionFee = 0;
            updatedAdoption.PaymentCollected = isAdopted;

            db.SubmitChanges();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            //Need to use clientID?
            Adoption animal = db.Adoptions.Where(a => a.AnimalId == animalId).Single();
            db.Adoptions.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            //find animal getting shot
            AnimalShot updatingShot = null;
            Shot shot = null;
            try
            {
                updatingShot = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Did not find matching AnimalID to the one entered.");
                return;
            }
            shot.Name = shotName;
            updatingShot.AnimalId = animal.AnimalId;
            updatingShot.ShotId = shot.ShotId;

            db.SubmitChanges();
        }
    }
}