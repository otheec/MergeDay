import PageBreadcrumb from "../components/common/PageBreadCrumb.tsx";
import {WorkspaceCard} from "../components/workspaces/WorkspacesCard.tsx";

const Workspaces = () => {
  return (
    <>
      <PageBreadcrumb pageTitle="Workspaces" />
      <div className="grid grid-flow-row grid-cols-1 sm:grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-4">
        <WorkspaceCard
          title="Orange team"
          description="Flip, Metalák, Omar, Řádím, Džony"
          color="orange"
          redirect={() => null}
        />
        <WorkspaceCard
          title="Pink team"
          description="This is a description"
          color="pink"
          redirect={() => null}
        />
        <WorkspaceCard
          title="Green team"
          description="This is a description"
          color="green"
          redirect={() => null}
        />
        <WorkspaceCard
          title="Yellow team"
          description="This is a description"
          color="yellow"
          redirect={() => null}
        />
      </div>
    </>
  )
}

export default Workspaces;
