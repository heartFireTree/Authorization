using MyTest.Utils;
using System;

namespace GoRun
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var jwt = JwtUtils.GenerateToken("abc", "BBQHCZlEixJzs1dXapEezYEtcMfuS3");
            var result = JwtUtils.CheckToken(jwt, "BBQHCZlEixJzs1dXapEezYEtcMfuS3");

            var resultInfo = JwtUtils.GetPayload("eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ0eXAiOiJKV1QiLCJzdWIiOiJhYmMiLCJpYXQiOjE2MDQzNzQ3NDAsImV4cCI6MTYwNDM4MTk0MCwidXNlcm5hbWUiOiJyb3kifQ.XXI07fLDK-m2gGvJe46VvR2OA4Orp3kA3AivCBrOS1l3I-Lxq3n2C_BCC5qjsz_fHAm2TQdhOM6Zdx6FRD4PJA");
        }
    }
}
