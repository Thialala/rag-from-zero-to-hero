import * as types from '../../types.bicep'

param openaiName string
param location string = resourceGroup().location
param openaiModelDeployments types.openaiModelDeployment[]
param roleAssignments types.roleAssignment[] = []

module openai 'br/public:avm/res/cognitive-services/account:0.4.1' = {
  name: 'openai'
  params: {
    kind: 'OpenAI'
    name: openaiName
    customSubDomainName: openaiName
    location: location
    roleAssignments: roleAssignments
  }
}

//Create the OpenAI account deployments (models) in a batchsize to avoid error
@batchSize(1)
resource resDeployments 'Microsoft.CognitiveServices/accounts/deployments@2023-10-01-preview' = [for openaiModelDeployment in openaiModelDeployments: {
  dependsOn: [openai]
  name: '${openaiName}/${openaiModelDeployment.name}'
  properties: {
    model: {
      format: 'OpenAI'
      name: openaiModelDeployment.modelName
      version: openaiModelDeployment.modelVersion
    }
  }
  sku: {
    name: openaiModelDeployment.skuName
    capacity: openaiModelDeployment.skuCapacity
  }
}]

output endpoint string = openai.outputs.endpoint
output name string = openai.outputs.name
