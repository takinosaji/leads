# leads

## Description
Ultimate task management and productivity tool. 

## Architecture
The application has to follow DDD-oriented application architecture patterns such as:
- Hexagonal Architecture
- Onion Architecture
- Clean Architecture by Robert Martin.

The Hexagonal Architecture is preferable due to the least range of restriction and flexibility to organize application core.

### Leads.Shell
Is the first adapter created for the Leads application. The idea is to distribute it as self-sufficient Docker container ready to use from the shells in different OS.

### Leads.Core
The core part of the application which contains:
- Domain Models and Services
- Primary and Secondary Ports

## Domain
### Core Concepts

#### Stream

#### Trail

#### Lead

#### Tag

#### Config