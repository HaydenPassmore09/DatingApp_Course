using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;

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
        Task<PagedList<User>> GetUsers(UserParams userParams);
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
        /*
        * Returns a like entity given the liker's userId and the likee's (recipientID), Returns null if the like doesn't exist
        */
        Task<Like> GetLike(int userId, int recipientId);
        /*
        * Returns a single message where the message id is equal to the id parameter passed into the methof
        */
        Task<Message> GetMessage(int id);
        /*
        * Returns the inbox, outbox or unread messages given what is specified in the message params
        */
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
        /*
        * Returns a conversation between two users given the users ids
        */
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
    }
}