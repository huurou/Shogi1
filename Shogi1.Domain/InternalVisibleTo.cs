using System.Runtime.CompilerServices;

// ApplicationがDomainのinternalメンバーを使用できるようにする
// Domainのどこかに配置する
[assembly: InternalsVisibleTo("Shogi1.Application")]