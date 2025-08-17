import { FC } from "react";
import { cva, type VariantProps } from "class-variance-authority";

const cardVariants = cva(
  "rounded-2xl border flex flex-col text-start p-5 md:p-6 select-none hover:cursor-pointer",
  {
    variants: {
      color: {
        orange: "border-orange-300 bg-orange-100 dark:border-orange-800 dark:bg-orange-900/30 text-orange-600 dark:text-orange-600",
        green: "border-green-300 bg-green-100 dark:border-green-800 dark:bg-green-900/30 text-green-600 dark:text-green-600",
        blue: "border-blue-300 bg-blue-100 dark:border-blue-800 dark:bg-blue-900/30 text-blue-600 dark:text-blue-600",
        yellow: "border-yellow-300 bg-yellow-100 dark:border-yellow-800 dark:bg-yellow-900/30 text-yellow-600 dark:text-yellow-600",
        pink: "border-pink-300 bg-pink-100 dark:border-pink-800 dark:bg-pink-900/30 text-pink-600 dark:text-pink-600",
        default: "border-gray-200 bg-white dark:border-gray-800 dark:bg-white/[0.03] text-black dark:text-white",
      },
    },
    defaultVariants: {
      color: "default",
    },
  }
);

type Props = {
  title: string;
  description?: string;
  redirect: () => void;
} & VariantProps<typeof cardVariants>;

export const WorkspaceCard: FC<Props> = ({ title, description, color, redirect }) => {
  return (
    <div onClick={redirect} className={cardVariants({ color })}>
      <h1 className="text-title-sm">{title}</h1>
      {description && <p className="text-sm font-light">{description}</p>}
    </div>
  );
};
