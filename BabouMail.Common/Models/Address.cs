namespace BabouMail.Common.Models
{
    /// <summary>
    /// Email Address
    /// </summary>
    public class Address
    {
        /// <summary>
        /// The Display Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Email Address
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Creates a new instance of Address
        /// </summary>
        public Address()
        {
        }

        /// <summary>
        /// Creates a new instance of Address with the emailAddress and optional name.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="name"></param>
        public Address(string emailAddress, string name = null)
        {
            EmailAddress = emailAddress;
            Name = name;
        }
    }
}
