using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JSQuery;
internal class LowCamelCasweAttributeLoader : CustomAttributeLoader {
    public override T[] LoadCustomAttributes<T>(ICustomAttributeProvider resource, bool inherit) {
        var declaredAttributes = base.LoadCustomAttributes<T>(resource, inherit);
        if (!declaredAttributes.Any() && typeof(T) == typeof(ScriptMemberAttribute) && resource is MemberInfo member) {
            var lowerCamelCaseName = char.ToLowerInvariant(member.Name[0]) + member.Name.Substring(1);
            return new[] { new ScriptMemberAttribute(lowerCamelCaseName) } as T[];
        }
        return declaredAttributes;
    }
}