using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Improbable.SpatialOS.Platform.Common;
using Improbable.SpatialOS.Deployment.V1Alpha1;
using Improbable.SpatialOS.PlayerAuth.V2Alpha1;
using Improbable.SpatialOS.ServiceAccount.V1Alpha1;
using Improbable.SpatialOS.Snapshot.V1Alpha1;

#pragma warning disable CS1998
namespace SpatialDLL.SDK
{
    public class ExternalMethods
    {

        public async Task<object> CreatePlayerIdentityToken(dynamic data)
        {
            return await Task.Run(() => {
                PlatformRefreshTokenCredential CredentialWithProvidedToken = new PlatformRefreshTokenCredential(data._RefreshToken);
                PlayerAuthServiceClient _playerAuthServiceClient = PlayerAuthServiceClient.Create(credentials: CredentialWithProvidedToken);
                var playerIdentityTokenResponse = _playerAuthServiceClient.CreatePlayerIdentityToken(
                    new CreatePlayerIdentityTokenRequest
                    {
                        Provider = data._Provider,
                        PlayerIdentifier = data._PlayerIdentifier,
                        ProjectName = data._ProjectName
                    });
                return playerIdentityTokenResponse.PlayerIdentityToken;
            });
        }
        
        public async Task<object> GetDeploymentIDByTag(dynamic data)
        {
            return await Task.Run(() => {
                PlatformRefreshTokenCredential CredentialWithProvidedToken = new PlatformRefreshTokenCredential(data._RefreshToken);
                DeploymentServiceClient _deploymentServiceClient = DeploymentServiceClient.Create(credentials: CredentialWithProvidedToken);
                var suitableDeployment = _deploymentServiceClient.ListDeployments(new ListDeploymentsRequest
                {
                    ProjectName = data._ProjectName,
                    DeploymentName = data._DeploymentName
                }).First(d => d.Tag.Contains(data._ScenarioDeploymentTag));

                return suitableDeployment.Id;
            });
        }
        public async Task<object> CreateLoginToken(dynamic data)
        {
            return await Task.Run(() => {
                PlatformRefreshTokenCredential CredentialWithProvidedToken = new PlatformRefreshTokenCredential(data._RefreshToken);
                PlayerAuthServiceClient _playerAuthServiceClient = PlayerAuthServiceClient.Create(credentials: CredentialWithProvidedToken);
                var createLoginTokenResponse = _playerAuthServiceClient.CreateLoginToken(
                    new CreateLoginTokenRequest
                    {
                        PlayerIdentityToken = data._PlayerIdentityToken,
                        DeploymentId = data._DeploymentID,
                        LifetimeDuration = Duration.FromTimeSpan(TimeSpan.FromSeconds(data._Duration)),
                        WorkerType = data._ScenarioWorkerType
                    });
                return createLoginTokenResponse.LoginToken;
            });
        }
        public async Task<object> ChangeDeploymentCapacity(dynamic data)
        {
            return await Task.Run(() => {
                PlatformRefreshTokenCredential CredentialWithProvidedToken = new PlatformRefreshTokenCredential(data._RefreshToken);
                DeploymentServiceClient _deploymentServiceClient = DeploymentServiceClient.Create(credentials: CredentialWithProvidedToken);
                var suitableDeployment = _deploymentServiceClient.ListDeployments(new ListDeploymentsRequest
                {
                    ProjectName = data._ProjectName
                }).First(d => d.Id.Contains(data._DeploymentID));

                suitableDeployment.WorkerConnectionCapacities.Clear();
                suitableDeployment.WorkerConnectionCapacities.Add(new WorkerCapacity
                {
                    WorkerType = data._ScenarioWorkerType,
                    MaxCapacity = data._DeploymentCapacity
                });
                _deploymentServiceClient.UpdateDeployment(new UpdateDeploymentRequest { Deployment = suitableDeployment });
                return suitableDeployment.Id;
            });
        }
        public async Task<object> CloneDeployment(dynamic data)
        {
            return await Task.Run(() => {
                PlatformRefreshTokenCredential CredentialWithProvidedToken = new PlatformRefreshTokenCredential(data._RefreshToken);
                DeploymentServiceClient _deploymentServiceClient = DeploymentServiceClient.Create(credentials: CredentialWithProvidedToken);
                var suitableDeployment = _deploymentServiceClient.ListDeployments(new ListDeploymentsRequest
                {
                    ProjectName = data._ProjectName
                }).First(d => d.Id.Contains(data._DeploymentID));

                var newDeployment = suitableDeployment.Clone();
                newDeployment.StartingSnapshotId = data._SnapshotID;
                newDeployment.Tag.Clear();
                newDeployment = _deploymentServiceClient.CreateDeployment(new CreateDeploymentRequest { Deployment = newDeployment })
                    .PollUntilCompleted()
                    .GetResultOrNull();
                newDeployment.Tag.Add(data._ScenarioDeploymentTag);
                _deploymentServiceClient.UpdateDeployment(new UpdateDeploymentRequest { Deployment = newDeployment });
                return newDeployment.Id;
            });
        }
        public async Task<object> StopDeployment(dynamic data)
        {
            return await Task.Run(() => {
                PlatformRefreshTokenCredential CredentialWithProvidedToken = new PlatformRefreshTokenCredential(data._RefreshToken);
                DeploymentServiceClient _deploymentServiceClient = DeploymentServiceClient.Create(credentials: CredentialWithProvidedToken);
                _deploymentServiceClient.StopDeployment(new StopDeploymentRequest
                {
                    Id = data._DeploymentID,
                    ProjectName = data._ProjectName
                });
                return data._DeploymentID;
            });
        }
        public async Task<object> CreateDeployment(dynamic data)
        {
            return await Task.Run(() => {
                PlatformRefreshTokenCredential CredentialWithProvidedToken = new PlatformRefreshTokenCredential(data._RefreshToken);
                DeploymentServiceClient _deploymentServiceClient = DeploymentServiceClient.Create(credentials: CredentialWithProvidedToken);
                var newDeployment = _deploymentServiceClient.CreateDeployment(new CreateDeploymentRequest
                {
                    Deployment = new Deployment
                    {
                        ProjectName = data._ProjectName,
                        Name = data._DeploymentName,
                        LaunchConfig = new LaunchConfig
                        {
                            ConfigJson = data._Config
                        },
                        AssemblyId = data._AssemblyID,
                        Tag = { data._ScenarioDeploymentTag }

                    }
                }).PollUntilCompleted().GetResultOrNull();
                return newDeployment.Id;
            });
        }
        public async Task<object> SetDeploymentMaintenance(dynamic data)
        {
            return await Task.Run(() => {
                PlatformRefreshTokenCredential CredentialWithProvidedToken = new PlatformRefreshTokenCredential(data._RefreshToken);
                DeploymentServiceClient _deploymentServiceClient = DeploymentServiceClient.Create(credentials: CredentialWithProvidedToken);
                var suitableDeployment = _deploymentServiceClient.ListDeployments(new ListDeploymentsRequest
                {
                    ProjectName = data._ProjectName,
                    DeploymentName = data._DeploymentName
                }).First(d => d.Id.Contains(data._DeploymentID));
                suitableDeployment.Tag.Remove(data._ScenarioDeploymentTag);
                suitableDeployment.WorkerFlags.Add(new WorkerFlag
                {
                    Key = "maintenance",
                    Value = "true",
                    WorkerType = data._ScenarioWorkerType
                });
                _deploymentServiceClient.UpdateDeployment(new UpdateDeploymentRequest { Deployment = suitableDeployment });
                return suitableDeployment.Id;
            });
        }
        public async Task<object> TakeDeploymentSnapshot(dynamic data)
        {
            return await Task.Run(() => {
                PlatformRefreshTokenCredential CredentialWithProvidedToken = new PlatformRefreshTokenCredential(data._RefreshToken);
                SnapshotServiceClient _snapshotServiceClient = SnapshotServiceClient.Create(credentials: CredentialWithProvidedToken);
                var latestSnapshot = _snapshotServiceClient.TakeSnapshot(new TakeSnapshotRequest
                {
                    Snapshot = new Snapshot
                    {
                        ProjectName = data._ProjectName,
                        DeploymentName = data._DeploymentName
                    }
                }).PollUntilCompleted()
                    .GetResultOrNull();
                return latestSnapshot.Id;
            });
        }
        public async Task<object> CreateServiceAccount(dynamic data)
        {
            return await Task.Run(() => {
                PlatformRefreshTokenCredential CredentialWithProvidedToken = new PlatformRefreshTokenCredential(data._RefreshToken);
                ServiceAccountServiceClient _serviceAccountServiceClient = ServiceAccountServiceClient.Create(credentials: CredentialWithProvidedToken);
                var perm = new Permission
                {
                    Parts = {
                    new RepeatedField<string> {"prj", data._ProjectName, "*"}
                }
                };
                if (data._WritePermission)
                {
                    perm.Verbs.Add(Permission.Types.Verb.Write);
                }
                if (data._ReadPermission)
                {
                    perm.Verbs.Add(Permission.Types.Verb.Read);
                }
                if (data._GrantPermission)
                {
                    perm.Verbs.Add(Permission.Types.Verb.Grant);
                }

                var _newAccount = _serviceAccountServiceClient.CreateServiceAccount(new CreateServiceAccountRequest
                {
                    Name = data._ServiceAccountName,
                    ProjectName = data._ProjectName,
                    Permissions = { new RepeatedField<Permission> { perm } },
                    Lifetime = Duration.FromTimeSpan(TimeSpan.FromSeconds(data._Lifetime))
                });
                return _newAccount.Id;
            });
        }
        public async Task<object> GetServiceAccountByName(dynamic data)
        {
            return await Task.Run(() => {
                PlatformRefreshTokenCredential CredentialWithProvidedToken = new PlatformRefreshTokenCredential(data._RefreshToken);
                ServiceAccountServiceClient _serviceAccountServiceClient = ServiceAccountServiceClient.Create(credentials: CredentialWithProvidedToken);
                var _account = _serviceAccountServiceClient.ListServiceAccounts(new ListServiceAccountsRequest
                {
                    ProjectName = data._ProjectName
                }).Where(account => account.Name == data._ServiceAccountName);
                if (_account.Count() > 0)
                {
                    return _account.First().Id.ToString();
                }
                else
                {
                    return null;
                }
            });
        }
        public async Task<object> IncreaseServiceAccountLifetime(dynamic data)
        {
            return await Task.Run(() => {
                PlatformRefreshTokenCredential CredentialWithProvidedToken = new PlatformRefreshTokenCredential(data._RefreshToken);
                ServiceAccountServiceClient _serviceAccountServiceClient = ServiceAccountServiceClient.Create(credentials: CredentialWithProvidedToken);
                var _account = _serviceAccountServiceClient.ListServiceAccounts(new ListServiceAccountsRequest
                {
                    ProjectName = data._ProjectName
                }).Where(account => account.Id == data._ServiceAccountID);
                if (_account.Count() > 0)
                {
                    _serviceAccountServiceClient.UpdateServiceAccount(new UpdateServiceAccountRequest
                    {
                        Id = _account.First().Id,
                        ExpirationTime = DateTime.UtcNow.AddSeconds(data._Lifeitime).ToTimestamp()
                    });
                    return _account.First().Id.ToString();
                }
                else
                {
                    return null;
                }
            });
        }
        public async Task<object> DeleteServiceAccount(dynamic data)
        {
            return await Task.Run(() => {
                PlatformRefreshTokenCredential CredentialWithProvidedToken = new PlatformRefreshTokenCredential(data._RefreshToken);
                ServiceAccountServiceClient _serviceAccountServiceClient = ServiceAccountServiceClient.Create(credentials: CredentialWithProvidedToken);
                _serviceAccountServiceClient.DeleteServiceAccount(new DeleteServiceAccountRequest
                {
                    Id = data._ServiceAccountID
                });
                return data._ServiceAccountID;
            });
        }
    }
}
