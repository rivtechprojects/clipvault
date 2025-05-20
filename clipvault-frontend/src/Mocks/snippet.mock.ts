export const snippetCollectionsMock = [
  {
    name: 'Git',
    snippets: [
      {
        title: 'Git Clone',
        language: 'shell',
        code: 'git clone <repo-url>',
        tags: ['git', 'clone'],
        description: 'Clone a repository.'
      },
      {
        title: 'Git Commit',
        language: 'shell',
        code: 'git commit -m "Your commit message"',
        tags: ['git', 'commit'],
        description: 'Commit staged changes with a message.'
      },
      {
        title: 'Git Pull',
        language: 'shell',
        code: 'git pull origin main',
        tags: ['git', 'pull'],
        description: 'Pull latest changes from main branch.'
      },
      {
        title: 'Git Status',
        language: 'shell',
        code: 'git status',
        tags: ['git', 'status'],
        description: 'Show the working tree status.'
      }
    ]
  },
  {
    name: 'Docker',
    snippets: [
      {
        title: 'Docker Run',
        language: 'shell',
        code: 'docker run -it ubuntu',
        tags: ['docker', 'run'],
        description: 'Run an Ubuntu container.'
      },
      {
        title: 'Docker Build',
        language: 'shell',
        code: 'docker build -t my-image .',
        tags: ['docker', 'build'],
        description: 'Build a Docker image from a Dockerfile.'
      },
      {
        title: 'Docker List Containers',
        language: 'shell',
        code: 'docker ps -a',
        tags: ['docker', 'list'],
        description: 'List all containers.'
      },
      {
        title: 'Docker Remove Container',
        language: 'shell',
        code: 'docker rm <container-id>',
        tags: ['docker', 'remove'],
        description: 'Remove a container by ID.'
      }
    ]
  },
  {
    name: 'TypeScript',
    snippets: [
      {
        title: 'TypeScript Interface',
        language: 'typescript',
        code: 'interface User {\n  id: number;\n  name: string;\n}',
        tags: ['typescript', 'interface'],
        description: 'A basic TypeScript interface.'
      },
      {
        title: 'TypeScript Function',
        language: 'typescript',
        code: 'function greet(name: string): string {\n  return `Hello, ${name}`;\n}',
        tags: ['typescript', 'function'],
        description: 'A simple TypeScript function.'
      },
      {
        title: 'TypeScript Enum',
        language: 'typescript',
        code: 'enum Color {\n  Red,\n  Green,\n  Blue\n}',
        tags: ['typescript', 'enum'],
        description: 'A basic TypeScript enum.'
      },
      {
        title: 'TypeScript Class',
        language: 'typescript',
        code: 'class Person {\n  constructor(public name: string) {}\n}',
        tags: ['typescript', 'class'],
        description: 'A simple TypeScript class.'
      }
    ]
  }
  // ...add more collections as needed...
];