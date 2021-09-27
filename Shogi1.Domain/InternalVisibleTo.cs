using System.Runtime.CompilerServices;

// ApplicationがDomainのinternalメンバーを使用できるようにする
// Domainのどこかに配置する
[assembly: InternalsVisibleTo("Shigi1.Application")]