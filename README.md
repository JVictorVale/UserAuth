<h4 align="center"> 
	🚧 Em construção...  🚧
</h4>

# <p align="center"> Api de Autenticação de Usuários </p>


## Descrição do Projeto:
Desenvolver um sistema de autenticação de usuário, permitindo que o usuário realize o cadastro, login e atualização do seu perfil.

*Obs.: Coloquei outras opções de rotas como ObterPorId e ObterTodos para integração do projeto da API com o FrontEnd*

*Obs².: Irei continuar a construção da API utilizando recuperação de senha com e-mail e pertimir que o usuário coloque sua foto*

### Descrição de Entidade:

* Usuario
  * Id (int)
  * Nome (Required, varchar(150))
  * Cpf (Required, varchar(11))
  * Email (Required, varchar(100))
  * Senha (Required, varchar(255))
  * CriadoEm (Required, DateTime)
  * AtualizadoEm (Required, DateTime)
 
### Registro do usuário

  * Pedir Nome, Email, Cpf, Senha.
  * Deve ser verificado se Email já esta em uso
  * O password deve ser armazenado utilizando algum algoritmo de hash (Argon2).

### 🛠 Tecnologias
- [.NET 6](https://dotnet.microsoft.com/pt-br/download/dotnet/6.0)
- [Entity Framework 6](https://learn.microsoft.com/pt-br/ef/ef6/)
- [MySQL](https://www.mysql.com/)
- [AutoMapper](https://automapper.org/)
- [FluentValidation](https://docs.fluentvalidation.net/en/latest/)
- [ScottBrady91.AspNetCore.Identity.Argon2PasswordHasher](https://github.com/scottbrady91/ScottBrady91.AspNetCore.Identity.Argon2PasswordHasher)

*  ### Autor
---

Feito por Victor Vale 👋🏽 Entre em contato!

[![Linkedin Badge](https://img.shields.io/badge/-Victor-blue?style=flat-square&logo=Linkedin&logoColor=white&link=https://www.linkedin.com/in/jvictorvale/)](https://www.linkedin.com/in/jvictorvale/) 
[![Gmail Badge](https://img.shields.io/badge/-joaovictorvale.dev@gmail.com-c14438?style=flat-square&logo=Gmail&logoColor=white&link=mailto:joaovictorvale.dev@gmail.com)](mailto:joaovictorvale.dev@gmail.com)
