namespace UserManagement
{
	public class UserManager
	{
		public string ManageUsers()
		{
			string[] users = { "Alice", "Bob" };
			return users[ 5 ]; // Index out of bounds, no error handling
		}
	}
}