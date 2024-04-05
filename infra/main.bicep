import * as types from 'types.bicep'

param openaiName string
param openaiModelDeployments types.openaiModelDeployment[]
param openaiRoleAssignments types.roleAssignment[] = []
param location string = resourceGroup().location

module openai 'core/ai/openai.bicep' = {
  name: 'deploy-openai'
  params: {
    openaiModelDeployments: openaiModelDeployments
    openaiName: openaiName
    location: location
    roleAssignments: openaiRoleAssignments
  }
}
