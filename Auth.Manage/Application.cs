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
            Inquirer.Go();
        }

        public void Menu()
        {
            Inquirer.Prompt(Question.Menu()
                .AddOption("Create New Client", () => CreateNewClient())
                .AddOption("Set Client Status", () => SetClientActiveStatus())
                .AddOption("Add Role", () => AddRole())
                .AddOption("Remove Role", () => RemoveRole())
                .AddOption("Add Role Claim", () => AddRoleClaim())
                .AddOption("Remove Role Claim", () => RemoveRoleClaim()));
        }

        private void RemoveRoleClaim()
        {
            UserRole role = null;
            var roles = _unitOfWork.RoleRepository.GetAllIncluding();

            Inquirer.Prompt(Question.List("Chose", roles).WithConvertToString(x => x.Name)).Bind(() => role);
            Inquirer.Prompt(() =>
            {
                var claims = _unitOfWork.RoleClaimRepository.GetAllIncluding();
                return Question.Checkbox("Chose", claims).WithConvertToString(x => x.ClaimValue);
            }).Then(answers =>
            {
                foreach (var claim in answers)
                {
                    _unitOfWork.RoleClaimRepository.Delete(claim);
                }

                _unitOfWork.SaveChanges();
                Menu();
            });
        }

        private void AddRoleClaim()
        {
            UserRole role = null;
            var roles = _unitOfWork.RoleRepository.GetAllIncluding();

            Inquirer.Prompt(Question.List("Chose", roles).WithConvertToString(x => x.Name)).Bind(() => role);
            Inquirer.Prompt(Question.Input("Name")).Then(answer =>
            {
                _unitOfWork.RoleClaimRepository.Insert(new RoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = "Permission",
                    ClaimValue = answer,
                });
                _unitOfWork.SaveChanges();
                Menu();
            });
        }

        private void RemoveRole()
        {
            Inquirer.Prompt(() =>
            {
                var roles = _unitOfWork.RoleRepository.GetAllIncluding();
                return Question.Checkbox("Remove", roles).WithConvertToString(x => x.Name);
            }).Then(answers =>
            {
                _unitOfWork.BeginTransaction();
                foreach (var role in answers)
                {
                    _unitOfWork.RoleClaimRepository.BatchDelete(rc => rc.RoleId == role.Id);
                    _unitOfWork.RoleRepository.BatchDelete(r => r.Id == role.Id);
                }
                _unitOfWork.Commit();
                Menu();
            });
        }

        private void AddRole()
        {
            var role = new UserRole();
            Inquirer.Prompt(Question.Input("Name")).Then(answer =>
            {
                role.Name = answer;
                _unitOfWork.RoleRepository.Insert(role);
                _unitOfWork.SaveChanges();
                Menu();
            });
        }

        private void SetClientActiveStatus()
        {
            var clients = _unitOfWork.ClientAPIRepository.GetAllIncluding().ToList();

            Inquirer.Prompt(Question.Checkbox("Alter", clients)
            .Page(10)
            .WithDefaultValue(item => { return clients.Where(c => c.IsActive).Any(c => c.Id == item.Id); })
            .WithConfirmation()
            .WithConvertToString(item => { return $"{item.Name}"; })).Then(answer =>
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
                Menu();
            });
        }

        private void CreateNewClient()
        {
            var client = new ClientAPI();
            Inquirer.Prompt(Question.Input("Id")).Bind(() => client.Id);
            Inquirer.Prompt(() => Question.Input("Name").WithDefaultValue(client.Id)).Bind(() => client.Name);
            Inquirer.Prompt(Question.Input("Allowed Origin")).Bind(() => client.AllowedOrigin);
            Inquirer.Prompt(Question.Input<int>("Refresh Token Lifetime (hours)")).Then(answer => client.RefreshTokenLifeTime = answer * 60);
            Inquirer.Prompt(Question.Confirm("Is Active")).Then(answer =>
            {
                client.IsActive = answer;
                client.ApplicationType = ApplicationType.JavaScript;
                client.Secret = "";

                _unitOfWork.ClientAPIRepository.Insert(client);
                _unitOfWork.SaveChanges();
                Menu();
            });
        }
    }
}