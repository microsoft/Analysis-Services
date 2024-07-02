export interface ComparisonNode {
 
  // Id of the object
  Id: number;

  // Node type of the object
  // Example: Data Source, Table, Relationship, KPI
  NodeType: string;

  // Id of this object's parent
  ParentId: number;

  // Name of this object in source schema
  SourceName: string;

  // Name of this object in target schema
  TargetName: string;

  // Name of this object in source schema
  SourceInternalName: string;

  // Name of this object in target schema
  TargetInternalName: string;

  // Indentation level of the object
  Level: number;

  // Status of the object compared to the source and target
  Status: string;

  // Current action to be performed for this object
  MergeAction: string;

  // Code at source
  SourceObjectDefinition: string;

  // Code at target
  TargetObjectDefinition: string;

  // Ids of the children nodes
  // Will be removed once all manipulation with child nodes is isolated to Comparison object
  ChildNodes: number[];

  // Actions that can be performed for this object
  AvailableActions: string[];

  // To maintain if the object is to be shown on UI or not
  ShowNode: boolean;

  // To maintain if the dropdown is disabled on the UI
  DropdownDisabled: boolean;
  // Text mentioning why the dropdown is disabled
  DisableMessage: string;

}