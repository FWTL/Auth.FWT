using System;
using System.Linq;
using Auth.FWT.Core.Data;
using Auth.FWT.Domain.Entities.API;
using Auth.FWT.Domain.Entities.Identity;
using InquirerCS;
using static Auth.FWT.Domain.Enums.Enum;

namespace Auth.Manage
{
    public class Application : IApplication
    {
        private IUnitOfWork _unitOfWork;

        public Application(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Run()
        {
            Menu();
        }

        public void Menu()
        {
            Question.Menu()
                .AddOption("Create New Client", () => CreateNewClient())
                .AddOption("Set Client Status", () => SetClientActiveStatus())
                .AddOption("Add Role", () => AddRole())
                .AddOption("Remove Role", () => RemoveRole())
                .AddOption("Add Role Claim", () => AddRoleClaim())
                .AddOption("Remove Role Claim", () => RemoveRoleClaim())
            .Prompt();
        }

        private void RemoveRoleClaim()
        {
            UserRole role = null;
            var roles = _unitOfWork.RoleRepository.GetAllIncluding();

            Question.Ask()
            .Then(() =>
            {
                Question.List("Chose", roles).WithConvertToString(x => x.Name).Then(answer =>
                {
                    role = answer;
                });
            })
            .Then(() =>
            {
                var claims = _unitOfWork.RoleClaimRepository.GetAllIncluding();

                Question.Checkbox("Chose", claims).WithConvertToString(x => x.ClaimValue).Then(answer =>
                {
                    foreach (var claim in answer)
                    {
                        _unitOfWork.RoleClaimRepository.Delete(claim);
                    }

                    _unitOfWork.SaveChanges();
                });
            }).Go();
        }

        private void AddRoleClaim()
        {
            UserRole role = null;
            var roles = _unitOfWork.RoleRepository.GetAllIncluding();

            Question.Ask()
            .Then(() =>
            {
                Question.List("Chose", roles).WithConvertToString(x => x.Name).Then(answer =>
                {
                    role = answer;
                });
            })
            .Then(() =>
            {
                Question.Input("Name").Then(answer =>
                {
                    _unitOfWork.RoleClaimRepository.Insert(new RoleClaim()
                    {
                        RoleId = role.Id,
                        ClaimType = "Permission",
                        ClaimValue = answer,
                    });
                    _unitOfWork.SaveChanges();
                });
            }).Go();
        }

        private void RemoveRole()
        {
            var roles = _unitOfWork.RoleRepository.GetAllIncluding();
            Question.Checkbox("Remove", roles).WithConvertToString(x => x.Name).Then(answer =>
             {
                 _unitOfWork.BeginTransaction();
                 foreach (var role in answer)
                 {
                     _unitOfWork.RoleClaimRepository.BatchDelete(rc => rc.RoleId == role.Id);
                     _unitOfWork.RoleRepository.BatchDelete(r => r.Id == role.Id);
                 }
                 _unitOfWork.Commit();
             });
        }

        private void AddRole()
        {
            var role = new UserRole();

            Question.Ask()
            .Then(() => { Question.Input("Name").Then(answer => role.Name = answer); })
            .Then(() =>
            {
                _unitOfWork.RoleRepository.Insert(role);
                _unitOfWork.SaveChanges();
            })
            .Go();
        }

        private void SetClientActiveStatus()
        {
            var clients = _unitOfWork.ClientAPIRepository.GetAllIncluding().ToList();

            Question.Checkbox("Alter", clients)
            .Page(10)
            .WithDefaultValue(item => { return clients.Where(c => c.IsActive).Any(c => c.Id == item.Id); })
            .WithConfirmation()
            .WithConvertToString(item => { return $"{item.Name}"; }).Then(answer =>
            {
                var toDiactivate = clients.Where(c => c.IsActive).Where(c => !answer.Any(a => a.Id == c.Id)).ToList();
                toDiactivate.ForEach(item =>
                {
                    item.IsActive = false;
                    _unitOfWork.ClientAPIRepository.Update(item);
                });

                var toActivate = clients.Where(c => !c.IsActive).Where(c => answer.Any(a => a.Id == c.Id)).ToList();
                toActivate.ForEach(item =>
                {
                    item.IsActive = true;
                    _unitOfWork.ClientAPIRepository.Update(item);
                });

                _unitOfWork.SaveChanges();
            });
        }

        private void CreateNewClient()
        {
            var client = new ClientAPI();

            Question.Ask()
            .Then(() => { Question.Input("Id").Then(answer => client.Id = answer); })
            .Then(() => { Question.Input("Name").WithDefaultValue(client.Id).Then(answer => client.Name = answer); })
            .Then(() => { Question.Input("Allowed Origin").WithDefaultValue("*").Then(answer => client.AllowedOrigin = answer); })
            .Then(() => { Question.Input<int>("Refresh Token Lifetime (hours)").WithValidation(answer => answer > 0, "answer > 0").Then(answer => client.RefreshTokenLifeTime = answer * 60); })
            .Then(() => { Question.Confirm("Is Active").Then(answer => client.IsActive = answer); })
            .Then(() =>
            {
                client.ApplicationType = ApplicationType.JavaScript;
                client.Secret = "";

                _unitOfWork.ClientAPIRepository.Insert(client);
                _unitOfWork.SaveChanges();
            })
            .Go();

            Menu();
        }
    }
}