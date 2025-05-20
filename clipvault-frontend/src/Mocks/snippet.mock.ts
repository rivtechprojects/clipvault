export const snippetMock = [
    {
        title: 'Async Await Example',
        language: 'javascript',
        code: 'async function fetchData() {\n  const res = await fetch(\'...\');\n  return res.json();\n}',
        tags: ['async', 'js', 'demo'],
        description: 'A simple async/await usage in JavaScript.'
    },
    {
        title: 'Python List Comprehension',
        language: 'python',
        code: '[x*x for x in range(10)]',
        tags: ['python', 'list', 'demo'],
        description: 'Quickly generate a list of squares in Python.'
    },
    {
        title: 'TypeScript Interface',
        language: 'typescript',
        code: 'interface User {\n  id: number;\n  name: string;\n}',
        tags: ['typescript', 'interface'],
        description: 'A basic TypeScript interface.'
    },
    {
        title: 'React Functional Component',
        language: 'javascript',
        code: 'function HelloWorld() {\n  return <div>Hello, World!</div>;\n}',
        tags: ['react', 'component'],
        description: 'A simple React functional component.'
    },
    {
        title: 'C# Hello World',
        language: 'csharp',
        code: 'Console.WriteLine("Hello, World!");',
        tags: ['csharp', 'hello'],
        description: 'Prints Hello World in C#.'
    },
    {
        title: 'Java For Loop',
        language: 'java',
        code: 'for (int i = 0; i < 10; i++) {\n  System.out.println(i);\n}',
        tags: ['java', 'loop'],
        description: 'A basic for loop in Java.'
    },
    {
        title: 'SQL Select All',
        language: 'sql',
        code: 'SELECT * FROM users;',
        tags: ['sql', 'select'],
        description: 'Selects all records from users table.'
    },
    {
        title: 'Bash Print Working Directory',
        language: 'bash',
        code: 'pwd',
        tags: ['bash', 'shell'],
        description: 'Prints the current working directory.'
    },
    {
        title: 'Ruby Each Iterator',
        language: 'ruby',
        code: '[1,2,3].each { |n| puts n }',
        tags: ['ruby', 'iterator'],
        description: 'Iterates over an array in Ruby.'
    },
    {
        title: 'Go HTTP Server',
        language: 'go',
        code: 'http.ListenAndServe(":8080", nil)',
        tags: ['go', 'http'],
        description: 'Starts a basic HTTP server in Go.'
    },
    {
        title: 'PHP Echo',
        language: 'php',
        code: '<?php echo "Hello, World!"; ?>',
        tags: ['php', 'echo'],
        description: 'Outputs Hello World in PHP.'
    },
    {
        title: 'Swift Variable Declaration',
        language: 'swift',
        code: 'var name: String = "Alice"',
        tags: ['swift', 'variable'],
        description: 'Declares a variable in Swift.'
    },
    {
        title: 'Kotlin Data Class',
        language: 'kotlin',
        code: 'data class User(val id: Int, val name: String)',
        tags: ['kotlin', 'data class'],
        description: 'Defines a data class in Kotlin.'
    },
    {
        title: 'Rust Println',
        language: 'rust',
        code: 'println!("Hello, world!");',
        tags: ['rust', 'println'],
        description: 'Prints Hello World in Rust.'
    },
    {
        title: 'HTML Basic Structure',
        language: 'html',
        code: '<!DOCTYPE html>\n<html>\n<head><title>Title</title></head>\n<body></body>\n</html>',
        tags: ['html', 'structure'],
        description: 'Basic HTML document structure.'
    },
    {
        title: 'CSS Center Div',
        language: 'css',
        code: '.center {\n  display: flex;\n  justify-content: center;\n  align-items: center;\n}',
        tags: ['css', 'center'],
        description: 'Centers a div using Flexbox.'
    },
    {
        title: 'Vue.js Data Property',
        language: 'javascript',
        code: 'data() {\n  return { message: "Hello Vue!" };\n}',
        tags: ['vue', 'data'],
        description: 'Defines a data property in Vue.js.'
    },
    {
        title: 'Dart Main Function',
        language: 'dart',
        code: 'void main() {\n  print("Hello, Dart!");\n}',
        tags: ['dart', 'main'],
        description: 'Entry point in Dart.'
    },
    {
        title: 'Perl Print',
        language: 'perl',
        code: 'print "Hello, Perl!\\n";',
        tags: ['perl', 'print'],
        description: 'Prints Hello in Perl.'
    },
    {
        title: 'Scala Map Example',
        language: 'scala',
        code: 'List(1,2,3).map(_ * 2)',
        tags: ['scala', 'map'],
        description: 'Doubles each element in a list.'
    },
    {
        title: 'Matlab Plot',
        language: 'matlab',
        code: 'plot(x, y)',
        tags: ['matlab', 'plot'],
        description: 'Plots y versus x in Matlab.'
    },
    {
        title: 'R Data Frame',
        language: 'r',
        code: 'df <- data.frame(x=1:3, y=c("a","b","c"))',
        tags: ['r', 'dataframe'],
        description: 'Creates a data frame in R.'
    },
    {
        title: 'C Printf',
        language: 'c',
        code: '#include <stdio.h>\nprintf("Hello, C!\\n");',
        tags: ['c', 'printf'],
        description: 'Prints Hello in C.'
    },
    {
        title: 'C++ Vector Push',
        language: 'cpp',
        code: '#include <vector>\nstd::vector<int> v;\nv.push_back(1);',
        tags: ['cpp', 'vector'],
        description: 'Pushes an element to a vector in C++.'
    },
    {
        title: 'Shell List Files',
        language: 'bash',
        code: 'ls -la',
        tags: ['bash', 'ls'],
        description: 'Lists files in a directory.'
    },
    {
        title: 'PowerShell Get-Process',
        language: 'powershell',
        code: 'Get-Process',
        tags: ['powershell', 'process'],
        description: 'Lists running processes in PowerShell.'
    },
    {
        title: 'Objective-C NSLog',
        language: 'objective-c',
        code: '@import Foundation;\nNSLog(@"Hello, Objective-C!");',
        tags: ['objective-c', 'nslog'],
        description: 'Logs a message in Objective-C.'
    },
    {
        title: 'Elixir Pipe Operator',
        language: 'elixir',
        code: '[1,2,3] |> Enum.map(&(&1 * 2))',
        tags: ['elixir', 'pipe'],
        description: 'Doubles each element using pipe operator.'
    },
    {
        title: 'Haskell List Filter',
        language: 'haskell',
        code: 'filter (> 5) [1..10]',
        tags: ['haskell', 'filter'],
        description: 'Filters list elements greater than 5.'
    },
    {
        title: 'Lua Table',
        language: 'lua',
        code: 't = {key = "value"}',
        tags: ['lua', 'table'],
        description: 'Creates a table in Lua.'
    },
    {
        title: 'JSON Example',
        language: 'json',
        code: '{ "name": "Alice", "age": 30 }',
        tags: ['json', 'example'],
        description: 'A simple JSON object.'
    }
];