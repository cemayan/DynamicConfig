using System;

namespace DynamicConfig.Core.Helpers
{
  public class RandomStrings
    {
       private int _id = 5;

      public RandomStrings(int id)
      {
        _id = id ;
      }

      public string generateString()
      {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[_id];
        var random = new Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

      return new String(stringChars);
      }
      
    }
}
