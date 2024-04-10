import * as types from '../../types.bicep'


param docIntelligenceName string
param location string = resourceGroup().location
param roleAssignments types.roleAssignment[] = []



module docIntelligence 'br/public:avm/res/cognitive-services/account:0.4.1' = {
  name: 'doc-intelligence'
  params: {
    kind: 'FormRecognizer'
    name: docIntelligenceName
    customSubDomainName: docIntelligenceName
    location: location
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
      ipRules: []
      virtualNetworkRules: []
    }
    disableLocalAuth: false
    roleAssignments: roleAssignments
  }
}
