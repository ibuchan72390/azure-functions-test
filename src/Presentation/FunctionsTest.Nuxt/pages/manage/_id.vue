<template>
  <manage-person :initId="personId"></manage-person>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'nuxt-property-decorator'

@Component
export default class Manage extends Vue {
  public get personId(): string {
    return this.$route.params.id
  }
}
</script>
