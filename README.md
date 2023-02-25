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
Is the first set of CLI-based adapters the Leads Application Core. 

The idea is to distribute it as self-sufficient Docker container ready to use from the shells in different OS.

### Leads.Core
This library contains Leads Application Core:
- Domain Models and Logic
- Primary Ports and Use-Cases

### Leads.SecondaryPorts
This library contains all secondary ports for the Leads Application Core located in _Leads.Core_.

## Domain
### Core Concepts

#### Forest

#### Trail

#### Lead

#### Tag

#### Config