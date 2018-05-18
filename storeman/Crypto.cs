using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace storeman
{ 
    class Crypto
    {
        private string input;
        public string Input
        {
          //  get { return input; }
            set { input = value; }

        }

        private string output;
        public string Output
        {
            get { return output; }
        //    set { output = value; }
        }

        private string message = null;
        public string Message
        {
            get { return message; }
         //   set { message = value; }
        }

        public void Encode()
        {
            try
            {
                byte[] encData_byte = new byte[input.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(input);
                output = Convert.ToBase64String(encData_byte);
            }
            catch (Exception e)
            {
                message = e.Message;
            }
        }

        public void Decode()
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecode_byte = Convert.FromBase64String(input);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                output = new String(decoded_char);
            }

            catch (Exception e)
            {
                message = e.Message;
            }

        }
    }
}
