using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptCs.Contracts;
using ScriptCs.Rebus;

namespace ScriptCs.S3
{
    public class S3ScriptPack : IScriptPack
    {
        public void Initialize(IScriptPackSession session)
        {
            Guard.AgainstNullArgument("session", session);

            session.ImportNamespace("Amazon");
            session.ImportNamespace("Amazon.S3");
            session.ImportNamespace("Amazon.S3.Model");
            session.ImportNamespace("Amazon.S3.Transfer");
        }

        public IScriptPackContext GetContext()
        {
            return new S3Script();
        }

        public void Terminate()
        {
        }
    }
}
