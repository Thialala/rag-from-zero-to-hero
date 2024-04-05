@export()
type openaiModelDeployment = {
  name: string
  modelName: string
  modelVersion: string
  skuName: string
  skuCapacity: int
}

@export()
type roleAssignment = {
  roleDefinitionIdOrName: string
  principalId: string
  principalType: 'ServicePrincipal' | 'Group' | 'User'
}
