export class DatabaseObjects {

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

    // Indentation level of the object
    Level: number;

    // Status of the object compared to the source and target
    Status: string;

    // Current action to be performed for this object
    SelectedAction: string;

    // Code at source
    SourceData: string;

    // Code at target
    TargetData: string;

    // Ids of the children nodes
    ChildNodes: number[];

    // Actions that can be performed for this object
    AvailableActions: string[];

    // To maintain if the object is to be shown on UI or not
    ShowNode: boolean;
}

