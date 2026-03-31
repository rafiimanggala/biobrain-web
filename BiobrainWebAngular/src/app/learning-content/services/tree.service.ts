import { FlatTreeControl } from '@angular/cdk/tree';
import { Injectable } from '@angular/core';
import { MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material/tree';
import { CurrentUserService } from 'src/app/auth/services/current-user.service';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { ContentTreeRow } from 'src/app/learning-content/services/learning-content-db';
import { LearningContentProviderService } from 'src/app/learning-content/services/learning-content-provider.service';

import { hasValue } from '../../share/helpers/has-value';
import { NodeModel } from '../models/node.model';

@Injectable({ providedIn: 'root' })
export class TreeService {
  flatTreeControl: FlatTreeControl<NodeModel>;
  flatDataSource: MatTreeFlatDataSource<NodeModel, NodeModel>;
  treeFlattener: MatTreeFlattener<NodeModel, NodeModel>;
  isDemoMode = false;

  private _courseId?: string;

  constructor(
    private readonly _routingService: RoutingService,
    private readonly _learningContentProvider: LearningContentProviderService,
    private readonly _userService: CurrentUserService,
    private readonly _appEvents: AppEventProvider,
  ) {
    this.flatTreeControl = new FlatTreeControl<NodeModel, NodeModel>(
      node => node.level ?? 0,
      node => node.expandable ?? false,
    );
    this.treeFlattener = new MatTreeFlattener(this._transformer,
      node => node.level ?? 0,
      node => node.expandable ?? false,
      node => node.children,
    );
    this.flatDataSource = new MatTreeFlatDataSource<NodeModel, NodeModel>(this.flatTreeControl, this.treeFlattener);
  }

  public hasNestedChild = (_: number, nodeData: NodeModel): boolean => {
    // Routing nodes - leaves
    if (nodeData.routing) return false;
    return nodeData.children && nodeData.children.length > 0;
  };

  public async initTree(courseId: string): Promise<void> {
    if (this.flatDataSource.data && this.flatDataSource.data.length > 0 && this._courseId === courseId) {
      return;
    }

    this._courseId = courseId;
    this.flatDataSource.data = await this._getMaterialsTree(courseId);

    // for (const node of this.flatDataSource.data) {
    //   if(node.autoExpand)
    //     this.flatTreeControl.expand(node);
    // }

    var user = await this._userService.user;
    this.isDemoMode = user?.isDemoSubscription() ?? true;
  }

  public getNode(id: string): NodeModel | undefined {
    const rootNodes = this.flatDataSource.data;
    const nodes: NodeModel[] = [];
    nodes.push(...rootNodes);
    rootNodes.forEach(x => this.flatTreeControl.getDescendants(x)?.forEach(y => nodes.push(y)));
    return nodes.find(n => n.entityId === id);
  }

  public showNode(nodeId: string): void {
    if (!this.flatDataSource.data || this.flatDataSource.data.length < 1) return;

    const node = this.getNode(nodeId);
    if (!hasValue(node)) return;

    let parent = this.getNode(node.parentId);
    while (hasValue(parent)) {
      this.flatTreeControl.expand(parent);
      parent = this.getNode(parent.parentId);
    }

    this._refreshTree();
  }

  private async _getMaterialsTree(courseId: string): Promise<NodeModel[]> {
    const tree = await this._learningContentProvider.getContentTreeForCourse(courseId);
    return tree
      .map(x => this._mapContentTreeRowToNodeModel(x))
      .sort((a, b) => a.order - b.order);
  }

  private _mapContentTreeRowToNodeModel(node: ContentTreeRow): NodeModel {
    const isRoutingNode = !node.contentTreeMeta.couldAddEntry && !node.contentTreeMeta.couldAddContent;
    return new NodeModel(
      node.nodeId,
      node.name,
      node.nodes?.map(x => this._mapContentTreeRowToNodeModel(x)).sort((a, b) => a.order - b.order) ?? [],
      node.parentId ?? '',
      node.order,
      // ToDo redo
      node.contentTreeMeta.depth === 1,
      node.contentTreeMeta.depth === 0 || node.contentTreeMeta.depth === 2,
      isRoutingNode
        ? this._routingService.getMaterialUrl(node.courseId, node.nodeId ?? 0)
        : '',        
      node.isAvailableInDemo,
      node.contentTreeMeta.autoExpand,
    );
  }

  private readonly _transformer = (node: NodeModel, level: number): NodeModel => {
    node.level = level;
    node.expandable = node.children && node.children.length > 0;
    return node;
  };

  private _refreshTree(): void {
    const { data } = this.flatDataSource;
    this.flatDataSource.data = [];
    this.flatDataSource.data = data;
  }
}
