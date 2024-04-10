import * as types from 'types.bicep'

param openaiName string
param openaiModelDeployments types.openaiModelDeployment[]
param openaiRoleAssignments types.roleAssignment[] = []
param location string = resourceGroup().location

param docIntelligenceName string
param docIntelligenceLocation string = location
param docIntelRoleAssignments types.roleAssignment[] = []


module openai 'core/ai/openai.bicep' = {
  name: 'deploy-openai'
  params: {
    openaiModelDeployments: openaiModelDeployments
    openaiName: openaiName
    location: location
    roleAssignments: openaiRoleAssignments
  }
}

module docIntelligence 'core/ai/doc-intelligence.bicep' = {
  name: 'deploy-docIntelligence'
  params: {
    docIntelligenceName: docIntelligenceName
    location: docIntelligenceLocation
    roleAssignments: docIntelRoleAssignments
  }
}
