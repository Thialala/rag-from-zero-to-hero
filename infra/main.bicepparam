using './main.bicep'

param location = 'francecentral' // Azure region
param openaiName = '<your-openai-name>' 
param openaiModelDeployments = [
  {
    name: 'gpt-4-turbo'
    modelName: 'gpt-4'
    modelVersion: '1106-Preview'
    skuCapacity: 40
    skuName: 'Standard'
  }
  {
    name: 'gpt-35-turbo'
    modelName: 'gpt-35-turbo'
    modelVersion: '1106'
    skuCapacity: 40
    skuName: 'Standard'
  }
  {
    name: 'text-embedding-ada-002'
    modelName: 'text-embedding-ada-002'
    modelVersion: '2'
    skuCapacity: 60
    skuName: 'Standard'
  }
]

param openaiRoleAssignments = [
  {
    principalId: '00000000-0000-0000-0000-000000000000'
    principalType: 'User'
    roleDefinitionIdOrName: 'Cognitive Services OpenAI Contributor'
  }
  {
    principalId: '00000000-0000-0000-0000-000000000000'
    principalType: 'Group'
    roleDefinitionIdOrName: 'Cognitive Services OpenAI User'
  }
]

param docIntelligenceName = '<your-doc-intelligence-name>'
param docIntelRoleAssignments = [
  {
    principalId: '00000000-0000-0000-0000-000000000000'
    principalType: 'User'
    roleDefinitionIdOrName: 'Cognitive Services User'
  }
]


