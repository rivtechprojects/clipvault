export const snippetCollectionsMock = [
  {
    id: 1,
    name: 'Git',
    snippets: [
      {
        id: 101,
        title: 'Git Clone',
        code: 'git clone <repo-url>',
        language: 'shell',
        tags: ['git', 'clone'],
        description: 'Clone a repository.',
        isDeleted: false
      },
      {
        id: 102,
        title: 'Git Commit',
        code: 'git commit -m "Your commit message"',
        language: 'shell',
        tags: ['git', 'commit'],
        description: 'Commit staged changes with a message.',
        isDeleted: false
      },
      {
        id: 103,
        title: 'Git Pull',
        code: 'git pull origin main',
        language: 'shell',
        tags: ['git', 'pull'],
        description: 'Pull latest changes from main branch.',
        isDeleted: false
      },
      {
        id: 104,
        title: 'Git Status',
        code: 'git status',
        language: 'shell',
        tags: ['git', 'status'],
        description: 'Show the working tree status.',
        isDeleted: false
      }
    ],
    parentCollectionId: null,
    subCollections: [],
    isDeleted: false
  },
  {
    id: 2,
    name: 'Docker',
    snippets: [
      {
        id: 201,
        title: 'Docker Run',
        code: 'docker run -it ubuntu',
        language: 'shell',
        tags: ['docker', 'run'],
        description: 'Run an Ubuntu container.',
        isDeleted: false
      },
      {
        id: 202,
        title: 'Docker Build',
        code: 'docker build -t my-image .',
        language: 'shell',
        tags: ['docker', 'build'],
        description: 'Build a Docker image from a Dockerfile.',
        isDeleted: false
      },
      {
        id: 203,
        title: 'Docker List Containers',
        code: 'docker ps -a',
        language: 'shell',
        tags: ['docker', 'list'],
        description: 'List all containers.',
        isDeleted: false
      },
      {
        id: 204,
        title: 'Docker Remove Container',
        code: 'docker rm <container-id>',
        language: 'shell',
        tags: ['docker', 'remove'],
        description: 'Remove a container by ID.',
        isDeleted: false
      }
    ],
    parentCollectionId: null,
    subCollections: [],
    isDeleted: false
  },
  {
    id: 3,
    name: 'TypeScript',
    snippets: [
      {
        id: 301,
        title: 'TypeScript Interface',
        code: 'interface User {\n  id: number;\n  name: string;\n}',
        language: 'typescript',
        tags: ['typescript', 'interface'],
        description: 'A basic TypeScript interface.',
        isDeleted: false
      },
      {
        id: 302,
        title: 'TypeScript Function',
        code: 'function greet(name: string): string {\n  return `Hello, ${name}`;\n}',
        language: 'typescript',
        tags: ['typescript', 'function'],
        description: 'A simple TypeScript function.',
        isDeleted: false
      }
    ],
    parentCollectionId: null,
    subCollections: [
      {
        id: 4,
        name: 'TypeScript Advanced',
        snippets: [
          {
            id: 401,
            title: 'TypeScript Enum',
            code: 'enum Color {\n  Red,\n  Green,\n  Blue\n}',
            language: 'typescript',
            tags: ['typescript', 'enum'],
            description: 'A basic TypeScript enum.',
            isDeleted: false
          },
          {
            id: 402,
            title: 'TypeScript Class',
            code: 'class Person {\n  constructor(public name: string) {}\n}',
            language: 'typescript',
            tags: ['typescript', 'class'],
            description: 'A simple TypeScript class.',
            isDeleted: false
          }
        ],
        parentCollectionId: 3,
        subCollections: [],
        isDeleted: false
      }
    ],
    isDeleted: false
  }
  // ...add more collections as needed...
];