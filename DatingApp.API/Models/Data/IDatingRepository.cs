using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.API.Models.Data
{
    public interface IDatingRepository
    {
        /* 
        * The generic T is so that we can use the same method for different entities 
        * For example we will use this method for adding a user and adding a photo
        */
        void Add<T>(T entity) where T : class;
        /*
        * The generic T is the same as the add method (Allowing us to delete both photos and users with one method)
        */
        void Delete<T>(T entity) where T : class;
        /*
        * Saves all changes in our database, returns true if there has been more than one save back to the database, returns false if not.
        */
        Task<bool> SaveAll();
        /*
        * Returns all users in the database
        */
        Task<IEnumerable<User>> GetUsers();
        /*
        * Returns an individual user from our database given the user id
        */
        Task<User> GetUser(int id);
        /*
        * Returns an individual users photo given the photo id
        */
        Task<Photo> GetPhoto(int id);
        /*
        * Returns a users main photo given the users Id
        */
        Task<Photo> GetMainPhotoForUser(int userId);

    }
}