<template>
  <select v-model="source">
    <option v-for="source of sources" :key="source.name" :value="source">{{source.name}}</option>
  </select>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, namespace } from 'nuxt-property-decorator'

import { ApiSourceStoreKeys, IApiSource } from '@/models'

const apiSource = namespace(ApiSourceStoreKeys.namespace)

@Component
export default class ApiSourceDropdown extends Vue {
  public get source() {
    return this.activeSource
  }

  public set source(source: IApiSource) {
    this.setSource(source)
  }

  @apiSource.Mutation(ApiSourceStoreKeys.mutations.setApiSource)
  setSource: (source: IApiSource) => void

  @apiSource.State(ApiSourceStoreKeys.state.apiSources)
  sources: IApiSource[]

  @apiSource.State(ApiSourceStoreKeys.state.activeApiSource)
  activeSource: IApiSource
}
</script>
