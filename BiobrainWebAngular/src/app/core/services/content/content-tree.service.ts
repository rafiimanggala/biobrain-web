import { Injectable } from '@angular/core';
import { from, Observable } from 'rxjs';
import { filter, map, shareReplay, tap } from 'rxjs/operators';

import { ContentTreeRow } from '../../../learning-content/services/learning-content-db';
import { LearningContentProviderService } from '../../../learning-content/services/learning-content-provider.service';
import { hasValue } from '../../../share/helpers/has-value';
import { toDictionary } from '../../../share/helpers/observable-operators';
import { toNonNullableWithError } from '../../../share/helpers/to-non-nullable';

import { ContentTreeNode } from './content-tree.node';
import { distinct } from 'src/app/share/helpers/distinct-arrays';
import { ContentTreeNodeMeta } from './content-tree.meta';

@Injectable({
  providedIn: 'root',
})
export class ContentTreeService {
  private readonly _flat = new Observable<ContentTreeNode[]>();
  private readonly _index = new Observable<Map<string, ContentTreeNode>>();
  private readonly _meta = new Observable<ContentTreeNodeMeta[]>();

  constructor(
    private readonly _learningContentProviderService: LearningContentProviderService,
  ) {
    this._flat = from(this._learningContentProviderService.getAllContentTreeRows()).pipe(
      map(rows => rows.map(_ => this._mapToContentTreeNode(_))),
      map(nodes => this._toFlat(nodes)),
      shareReplay(1),
    );

    this._index = this._flat.pipe(
      toDictionary(_ => _.row.nodeId),
      shareReplay(1),
    );

    this._meta = this._flat.pipe(
      map(rows => distinct(rows.map(_ => new ContentTreeNodeMeta(_.row.contentTreeMeta, _.row.courseId)), (a,b) => a.dbRow.contentTreeMetaId == b.dbRow.contentTreeMetaId)),
      // tap(meta => console.log(meta)),
      shareReplay(1),
    );
  }

  public providerForGetNodeById(): Observable<(contentTreeNodeId: string) => ContentTreeNode> {
    return this._index.pipe(
      map(index => (contentTreeNodeId: string) => toNonNullableWithError('Content tree node was not found. (provider)')(index.get(contentTreeNodeId))),
      shareReplay(1),
    );
  }

  public getNode(contentTreeNodeId: string): Observable<ContentTreeNode> {
    return this._index.pipe(
      map(index => index.get(contentTreeNodeId)),
      map(toNonNullableWithError('Content tree node was not found.')),
      shareReplay(1),
    );
  }

  public findNode(contentTreeNodeId: string): Observable<ContentTreeNode|undefined> {
    return this._index.pipe(
      map(index => index.get(contentTreeNodeId)),
      shareReplay(1),
    );
  }

  public getMeta(courseId: string): Observable<ContentTreeNodeMeta[] | undefined> {
    return this._meta.pipe(
      map(_ => _.filter(m => m.courseId == courseId)),
      shareReplay(1));
  }

  public getMetaValuesWithParent(metaId: string, parentId: string|null): Observable<ContentTreeNode[] | undefined> {
    return this._flat.pipe(
      map(_ => _.filter(m => m.row.contentTreeMeta.contentTreeMetaId == metaId)),
      map(_ => _.filter(m => parentId == null || m.row.parentId == parentId).sort((a,b) => a.order - b.order)),
      shareReplay(1));
  }

  public getFirstLevel(courseId: string): Observable<ContentTreeNode | undefined> {
    return this._flat.pipe(
      map(flat => {
        var firstNode = flat.filter(_ => _.row.courseId == courseId && !_.parent).sort((a,b) =>  a.order - b.order)[0];
        if (!firstNode) return undefined;
        while(hasValue(firstNode.children) && firstNode.children.length > 0){
          firstNode = firstNode.children.sort((a,b) => a.order - b.order)[0];
        }
        return firstNode;
      }),
      shareReplay(1))
  }

  private _mapToContentTreeNode(row: ContentTreeRow, pathToNode: ContentTreeNode[] = []): ContentTreeNode {
    const node = new ContentTreeNode(row, pathToNode);
    node.children = hasValue(row.nodes) ? row.nodes.sort((a, b) => a.order - b.order).map(_ => this._mapToContentTreeNode(_, [...pathToNode, node])) : [];
    return node;
  }

  private _toFlat(nodes: ContentTreeNode[]): ContentTreeNode[] {
    return nodes.reduce((acc, i) => acc.concat(this._toFlat(i.children)), nodes);
  }
}

