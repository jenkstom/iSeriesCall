using cwbx;
using Npgsql;

namespace iseriesexample
{
    public partial class mySearch : Form
    {
		private AS400System as400 = new AS400System();
		private cwbx.StringConverter sc = new cwbx.StringConverter();
		private PackedConverter pc = new PackedConverter();

		public partial class mySearch : Form
		{
			InitializeComponent();

			as400.Define("MY400");
			as400.UserID = "USER";
			as400.Password = "PASS";
			as400.Connect(cwbcoServiceEnum.cwbcoServiceRemoteCmd);

		}

        private bool CheckSwapSystem()
        {
            bool result = true;

            // define the params for the SWPQRYCHK call
            ProgramParameters parms = new ProgramParameters();
            parms.Append("DATA", cwbrcParameterTypeEnum.cwbrcOutput, 1);

            // Initialize the output param with empty data
            sc.Length = 1;
            parms["DATA"].Value = sc.ToBytes("");

            //call the program
            cwbx.Program pgm = new cwbx.Program();
            pgm.system = as400;
            pgm.LibraryName = "DL2017";
            pgm.ProgramName = "SWPQRYCHK";
            pgm.Call(parms);

            String response = sc.FromBytes(parms["DATA"].Value).Trim().ToUpper();

            if (response == "X")
            {
                result = false;
            }

            return result;
        }

        private String GetSwapQueryID(string unit1, string unit2="")
        {

            // define the params for the SWPQRY call
            ProgramParameters parms = new ProgramParameters();
            parms.Append("UNIT1", cwbrcParameterTypeEnum.cwbrcInput, 10);
            parms.Append("UNIT2", cwbrcParameterTypeEnum.cwbrcInput, 10);
            parms.Append("PROBLEM", cwbrcParameterTypeEnum.cwbrcInput, 30);
            parms.Append("MAXSWAPS", cwbrcParameterTypeEnum.cwbrcInput, 3);
            parms.Append("FEASIBILITY", cwbrcParameterTypeEnum.cwbrcInput, 1);
            parms.Append("RETURNID", cwbrcParameterTypeEnum.cwbrcOutput, 6);

            // Set the parameter
            sc.Length = 10;
            parms["UNIT1"].Value = sc.ToBytes(unit1);
            parms["UNIT2"].Value = sc.ToBytes(unit2);
            sc.Length = 30;
            parms["PROBLEM"].Value = sc.ToBytes("GOODSAMARITAN");
            sc.Length = 3;
            if (unit2.Trim() == "")
            {
                parms["MAXSWAPS"].Value = sc.ToBytes("1");
            }
            else
            {
                parms["MAXSWAPS"].Value = sc.ToBytes("10");
            }
            sc.Length = 1;
            parms["FEASIBILITY"].Value = sc.ToBytes("1");
            sc.Length = 6;
            parms["RETURNID"].Value = sc.ToBytes("");

            //call the program
            cwbx.Program pgm = new cwbx.Program();
            pgm.system = as400;
            pgm.LibraryName = "DL2017";
            pgm.ProgramName = "SWPQRY";
            
            pgm.Call(parms);

            string queryid = sc.FromBytes(parms["RETURNID"].Value).Trim().ToUpper();

            return queryid;

        }
        
	}
}