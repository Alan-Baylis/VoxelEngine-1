﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms.Pathfinding;
using Assets.Scripts.Data.Map;
using Assets.Scripts.Logic.Jobs;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class JobController : MonoBehaviour
    {
        protected readonly Dictionary<JobType, PriorityQueue<Job>> OpenJobs = new Dictionary<JobType, PriorityQueue<Job>>();
        protected readonly List<JobSolver> FreeSolvers = new List<JobSolver>();
        protected List<Job>[,,] Jobs;
        
        void Update()
        {
            foreach (var freeSolver in FreeSolvers.ToArray())
            {
                var possibleJobs = freeSolver.GetPossibleJobs();
                var jobType = possibleJobs.Dequeue();
                if (OpenJobs.ContainsKey(jobType) && !OpenJobs[jobType].IsEmpty())
                {
                    foreach (var job in OpenJobs[jobType])
                    {
                        if (!job.GetPossibleWorkLocations().Any())
                        {
                            continue;
                        }

                        freeSolver.Solve(OpenJobs[jobType].Dequeue(job));
                        FreeSolvers.Remove(freeSolver);
                        break;
                    }
                }
            }
        }

        public void AddJob(Job job)
        {
            if (Jobs == null)
            {
                if (!Map.Instance.IsDoneGenerating)
                    return;
                Jobs = new List<Job>[Map.Instance.MapData.Chunks.GetLength(0)*Chunk.ChunkSize, 
                                            Map.Instance.MapData.Chunks.GetLength(1) * Chunk.ChunkSize, 
                                            Map.Instance.MapData.Chunks.GetLength(2) * Chunk.ChunkSize];
            }
            if (!OpenJobs.ContainsKey(job.GetJobType()))
            {
                OpenJobs.Add(job.GetJobType(), new PriorityQueue<Job>());
            }
            OpenJobs[job.GetJobType()].Enqueue(job, 1);
            if (Jobs[(int) job.Position.x, (int) job.Position.y, (int) job.Position.z] == null)
            {
                Jobs[(int)job.Position.x, (int)job.Position.y, (int)job.Position.z] = new List<Job>();
            }
            Jobs[(int)job.Position.x, (int)job.Position.y, (int)job.Position.z].Add(job);
        }

        public void SolveJob(Job job)
        {
            Jobs[(int) job.Position.x, (int) job.Position.y, (int) job.Position.z].Remove(job);
        }

        public bool HasJob(Vector3 pos, JobType jobType)
        {
            if (Jobs == null || Jobs[(int) pos.x, (int) pos.y, (int) pos.z] == null)
                return false;
            return Jobs[(int) pos.x, (int) pos.y, (int) pos.z].Any(j => j.GetJobType().Equals(jobType));
        }

        public void AddIdleSolver(JobSolver solver)
        {
            FreeSolvers.Add(solver);
        }
    }
}
